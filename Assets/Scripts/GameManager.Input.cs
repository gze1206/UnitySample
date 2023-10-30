using System;
using UnityEngine;

public partial class GameManager
{
    private enum InputDirection
    {
        None,
        
        Left,
        Right,
        Up,
        Down,
    }
    
    private const float swipeThreshold = 50f;
    
    private Vector2 beginPosition;
    private bool isDragging;

    private InputDirection GetSwipeDirection()
    {
        if (Input.touches.Length > 0)
        {
            var touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    this.isDragging = true;
                    this.beginPosition = touch.position;
                    return InputDirection.None;
                
                case TouchPhase.Ended:
                    this.isDragging = false;
                    return this.GetDirWithVec2(touch.position);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.isDragging = true;
                this.beginPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0) && this.isDragging)
            {
                this.isDragging = false;
                return this.GetDirWithVec2(Input.mousePosition);
            }
        }

        return InputDirection.None;
    }

    private InputDirection GetDirWithVec2(Vector2 endPosition)
    {
        if (Vector2.Distance(this.beginPosition, endPosition) < swipeThreshold)
        {
            return InputDirection.None;
        }

        var dir = endPosition - this.beginPosition;
        if (MathF.Abs(dir.x) > MathF.Abs(dir.y))
        {
            return dir.x > 0
                ? InputDirection.Right
                : InputDirection.Left;
        }

        return dir.y > 0
            ? InputDirection.Up
            : InputDirection.Down;
    }

    private InputDirection GetInputDirection()
    {
        var swipe = this.GetSwipeDirection();

        if (swipe != InputDirection.None) return swipe;
        
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            return InputDirection.Left;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            return InputDirection.Right;
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            return InputDirection.Up;
        }
        
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            return InputDirection.Down;
        }

        return InputDirection.None;
    }
}
