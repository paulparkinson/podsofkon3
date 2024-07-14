using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace de.enjoyLife.Smoke
{
    public class Rotation : MonoBehaviour
    {

        public enum rotationAxis : byte
        {
            NONE = 0x00,
            X = 0x01,
            Y = 0x02,
            Z = 0x04,
            XY = 0x03,
            XZ = 0x05,
            YZ = 0x06,
            XYZ = 0x07
        }

        [SerializeField]
        private float rotationSpeed = 10f;
        [SerializeField]
        private rotationAxis rotAxis = rotationAxis.NONE;
        [SerializeField]
        private Space useWorldSpace = Space.World;
        private Vector3 RotationAxis = new Vector3(0,0,0);

        public float RotationSpeed
        {
            get
            {
                return rotationSpeed;
            }

            set
            {
                rotationSpeed = value;
            }
        }

        public rotationAxis RotAxis
        {
            get
            {
                return rotAxis;
            }

            set
            {
                rotAxis = value;
                changeRotationAxis(value);
            }
        }

        public Space UseWorldSpace
        {
            get
            {
                return useWorldSpace;
            }

            set
            {
                useWorldSpace = value;
            }
        }

        private void OnValidate()
        {
            changeRotationAxis(rotAxis);
        }

        /// <summary>
        /// Adds an axis to the rotation set.
        /// e.g. if X is the current axis and you use the parameter rotationAxis.Y then it will be rotationAxis.XY
        /// </summary>
        /// <param name="axis">The axis.</param>
        public void addAxis(rotationAxis axis)
        {
            RotAxis = RotAxis | axis;
        }

        /// <summary>
        /// Removes an axis from the rotation set.
        /// e.g. if XY is the current axis and you use the parameter rotationAxis.Y then it will be rotationAxis.X
        /// </summary>
        /// <param name="axis">The axis.</param>
        public void removeAxis(rotationAxis axis)
        {
            RotAxis = RotAxis & ~axis;
        }

        /// <summary>
        /// Checks if the axis given in the parameter is in the axis set. 
        /// e.g. if xy is set then it will return true for x but false for z
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <returns><c>true</c> if axis is set, <c>false</c> otherwise.</returns>
        public bool checkAxis(rotationAxis axis)
        {
            if((axis&RotAxis) == axis)
            {
                return true;
            }
            return false;
        }

        private void changeRotationAxis(rotationAxis axis)
        {
            switch(axis)
            {
                case rotationAxis.X:
                    RotationAxis = Vector3.right;
                    break;
                case rotationAxis.Y:
                    RotationAxis = Vector3.up;
                    break;
                case rotationAxis.Z:
                    RotationAxis = Vector3.forward;
                    break;
                case rotationAxis.XY:
                    RotationAxis = Vector3.right+Vector3.up;
                    break;
                case rotationAxis.XYZ:
                    RotationAxis = Vector3.right+Vector3.up+Vector3.forward;
                    break;
                case rotationAxis.XZ:
                    RotationAxis = Vector3.right+Vector3.forward;
                    break;
                case rotationAxis.YZ:
                    RotationAxis = Vector3.up+Vector3.forward;
                    break;
                case rotationAxis.NONE:
                    goto default;
                default:
                    RotationAxis = Vector3.zero;
                    break;

            }
        }

        void Start()
        {

        }

        void Update()
        {
            transform.Rotate(RotationAxis, rotationSpeed * Time.deltaTime, useWorldSpace);
        }
    }

}