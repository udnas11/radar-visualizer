using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraInputHandler : MonoBehaviour
{

    public event Action OnMouseEnterAction;
    public event Action OnMouseExitAction;
    public event Action<int> OnMouseButtonDownAction;
    public event Action<int, Vector2> OnMouseButtonDragAction; // mouse button, delta position
    public event Action<int> OnMouseButtonUpAction;

    #region public serialised vars
    [SerializeField]
    protected Camera _camera;
    [SerializeField]
    LayerMask _raycastLayers;
    #endregion


    #region private protected vars
    protected bool _isMouseOver;
    const int _mouseButtonsToTrack = 3;
    List<int> _mouseButtonsHeld = new List<int>();
    #endregion


    #region pub methods
    public Camera Camera { get { return _camera; } }
    public bool IsMouseOver { get { return _isMouseOver; } }
    public int LayerMask { get { return _raycastLayers.value; } }

    protected bool GetIsMouseOver()
    {
        Vector3 mPos = Input.mousePosition;
        mPos = _camera.ScreenToViewportPoint(mPos);
        return (mPos.x >= 0f && mPos.x <= 1f) && (mPos.y >= 0f && mPos.y <= 1f);
    }
    #endregion


    #region private protected methods
    protected virtual void OnMouseEnter()
    {
        if (OnMouseEnterAction != null)
            OnMouseEnterAction();

        GlobalInputHandler.Instance.SetActiveCamera(this);
    }

    protected virtual void OnMouseExit()
    {
        if (OnMouseExitAction != null)
            OnMouseExitAction();
    }

    protected virtual void OnMouseButtonDown(int mouseBtn)
    {
        if (OnMouseButtonDownAction != null)
            OnMouseButtonDownAction(mouseBtn);
    }

    protected virtual void OnMouseButtonUp(int mouseBtn)
    {
        if (OnMouseButtonUpAction != null)
            OnMouseButtonUpAction(mouseBtn);
    }

    protected virtual void OnMouseButtonDrag(int mouseBtn, Vector2 delta)
    {
        if (OnMouseButtonDragAction != null)
            OnMouseButtonDragAction(mouseBtn, delta);
    }
    #endregion


    #region events
    #endregion


    #region mono events
    protected virtual void Update()
    {
        bool newMouseOver = GetIsMouseOver();
        if (newMouseOver != _isMouseOver)
        {
            if (newMouseOver)
                OnMouseEnter();
            else
                OnMouseExit();
            _isMouseOver = newMouseOver;
        }

        for (int mouseBtn = 0; mouseBtn < _mouseButtonsToTrack; mouseBtn++)
        {
            if (Input.GetMouseButtonDown(mouseBtn) && _isMouseOver)
            {
                Assert.IsFalse(_mouseButtonsHeld.Contains(mouseBtn));
                _mouseButtonsHeld.Add(mouseBtn);
                OnMouseButtonDown(mouseBtn);
            }

            if (Input.GetMouseButton(mouseBtn) &&_mouseButtonsHeld.Contains(mouseBtn))
            {
                Vector2 delta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                OnMouseButtonDrag(mouseBtn, delta);
            }

            if (Input.GetMouseButtonUp(mouseBtn))
            {
                if (_mouseButtonsHeld.Contains(mouseBtn))
                {
                    _mouseButtonsHeld.Remove(mouseBtn);
                    OnMouseButtonUp(mouseBtn);
                }
            }
        }
    }
    #endregion

}
