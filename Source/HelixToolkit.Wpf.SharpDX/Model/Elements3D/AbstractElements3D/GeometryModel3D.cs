﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeometryModel3D.cs" company="Helix Toolkit">
//   Copyright (c) 2014 Helix Toolkit contributors
// </copyright>
// <summary>
//   Provides a base class for a scene model which contains geometry
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using Core;
    using global::SharpDX;
    using global::SharpDX.Direct3D11;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Linq;
    using HelixToolkit.Wpf.SharpDX.Model;
    using Model.Scene;
    /// <summary>
    /// Provides a base class for a scene model which contains geometry
    /// </summary>
    public abstract class GeometryModel3D : Element3D, IHitable, IThrowingShadow, ISelectable, IMouse3D
    {
        #region DependencyProperties        
        /// <summary>
        /// The geometry property
        /// </summary>
        public static readonly DependencyProperty GeometryProperty =
            DependencyProperty.Register("Geometry", typeof(Geometry3D), typeof(GeometryModel3D), new PropertyMetadata(null, 
                (d,e)=> 
                {
                    ((d as Element3DCore).SceneNode as NodeGeometry).Geometry = e.NewValue as Geometry3D;
                }));
        /// <summary>
        /// The depth bias property
        /// </summary>
        public static readonly DependencyProperty DepthBiasProperty =
            DependencyProperty.Register("DepthBias", typeof(int), typeof(GeometryModel3D), new PropertyMetadata(0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodeGeometry).DepthBias = (int)e.NewValue;
            }));
        /// <summary>
        /// The slope scaled depth bias property
        /// </summary>
        public static readonly DependencyProperty SlopeScaledDepthBiasProperty =
            DependencyProperty.Register("SlopeScaledDepthBias", typeof(double), typeof(GeometryModel3D), new PropertyMetadata(0.0, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodeGeometry).SlopeScaledDepthBias = (float)(double)e.NewValue;
            }));
        /// <summary>
        /// The is selected property
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(false));
        /// <summary>
        /// The is multisample enabled property
        /// </summary>
        public static readonly DependencyProperty IsMultisampleEnabledProperty =
            DependencyProperty.Register("IsMultisampleEnabled", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodeGeometry).IsMSAAEnabled = (bool)e.NewValue;
            }));
        /// <summary>
        /// The fill mode property
        /// </summary>
        public static readonly DependencyProperty FillModeProperty = DependencyProperty.Register("FillMode", typeof(FillMode), typeof(GeometryModel3D),
            new PropertyMetadata(FillMode.Solid, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodeGeometry).FillMode = (FillMode)e.NewValue;
            }));
        /// <summary>
        /// The is scissor enabled property
        /// </summary>
        public static readonly DependencyProperty IsScissorEnabledProperty =
            DependencyProperty.Register("IsScissorEnabled", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodeGeometry).IsScissorEnabled = (bool)e.NewValue;
            }));
        /// <summary>
        /// The enable view frustum check property
        /// </summary>
        public static readonly DependencyProperty EnableViewFrustumCheckProperty =
            DependencyProperty.Register("EnableViewFrustumCheck", typeof(bool), typeof(GeometryModel3D), new PropertyMetadata(true,
                (d, e) =>
                {
                    ((d as Element3DCore).SceneNode as NodeGeometry).EnableViewFrustumCheck = (bool)e.NewValue; }));
        /// <summary>
        /// The is depth clip enabled property
        /// </summary>
        public static readonly DependencyProperty IsDepthClipEnabledProperty = DependencyProperty.Register("IsDepthClipEnabled", typeof(bool), typeof(GeometryModel3D),
            new PropertyMetadata(true, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodeGeometry).IsDepthClipEnabled = (bool)e.NewValue;
            }));


        // Using a DependencyProperty as the backing store for PostEffects.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PostEffectsProperty =
            DependencyProperty.Register("PostEffects", typeof(string), typeof(GeometryModel3D), new PropertyMetadata("", (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodeGeometry).PostEffects = e.NewValue as string;
            }));

        public string PostEffects
        {
            get { return (string)GetValue(PostEffectsProperty); }
            set { SetValue(PostEffectsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the geometry.
        /// </summary>
        /// <value>
        /// The geometry.
        /// </value>
        public Geometry3D Geometry
        {
            get
            {
                return (Geometry3D)this.GetValue(GeometryProperty);
            }
            set
            {
                this.SetValue(GeometryProperty, value);
            }
        }

        /// <summary>
        /// List of instance matrix.
        /// </summary>
        public static readonly DependencyProperty InstancesProperty =
            DependencyProperty.Register("Instances", typeof(IList<Matrix>), typeof(GeometryModel3D), new PropertyMetadata(null, (d, e) =>
            {
                ((d as Element3DCore).SceneNode as NodeGeometry).Instances = e.NewValue as IList<Matrix>;
            }));

        /// <summary>
        /// List of instance matrix. 
        /// </summary>
        public IList<Matrix> Instances
        {
            get { return (IList<Matrix>)this.GetValue(InstancesProperty); }
            set { this.SetValue(InstancesProperty, value); }
        }

        /// <summary>
        /// Gets or sets the depth bias.
        /// </summary>
        /// <value>
        /// The depth bias.
        /// </value>
        public int DepthBias
        {
            get
            {
                return (int)this.GetValue(DepthBiasProperty);
            }
            set
            {
                this.SetValue(DepthBiasProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets the slope scaled depth bias.
        /// </summary>
        /// <value>
        /// The slope scaled depth bias.
        /// </value>
        public double SlopeScaledDepthBias
        {
            get
            {
                return (double)this.GetValue(SlopeScaledDepthBiasProperty);
            }
            set
            {
                this.SetValue(SlopeScaledDepthBiasProperty, value);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Only works under FillMode = Wireframe. MSAA is determined by viewport MSAA settings for FillMode = Solid
        /// </summary>
        public bool IsMultisampleEnabled
        {
            set
            {
                SetValue(IsMultisampleEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsMultisampleEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets the fill mode.
        /// </summary>
        /// <value>
        /// The fill mode.
        /// </value>
        public FillMode FillMode
        {
            set
            {
                SetValue(FillModeProperty, value);
            }
            get
            {
                return (FillMode)GetValue(FillModeProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is scissor enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is scissor enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsScissorEnabled
        {
            set
            {
                SetValue(IsScissorEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsScissorEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is depth clip enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is depth clip enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDepthClipEnabled
        {
            set
            {
                SetValue(IsDepthClipEnabledProperty, value);
            }
            get
            {
                return (bool)GetValue(IsDepthClipEnabledProperty);
            }
        }
        /// <summary>
        /// Gets or sets a value indicating whether [enable view frustum check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [enable view frustum check]; otherwise, <c>false</c>.
        /// </value>
        public bool EnableViewFrustumCheck
        {
            set
            {
                SetValue(EnableViewFrustumCheckProperty, value);
            }
            get
            {
                return (bool)GetValue(EnableViewFrustumCheckProperty);
            }
        }
        #endregion     

        protected override void AssignDefaultValuesToSceneNode(SceneNode node)
        {
            if(node is NodeGeometry n)
            {
                n.DepthBias = this.DepthBias;
                n.IsDepthClipEnabled = this.IsDepthClipEnabled;
                n.SlopeScaledDepthBias = (float)this.SlopeScaledDepthBias;
                n.IsMSAAEnabled = this.IsMultisampleEnabled;
                n.FillMode = this.FillMode;
                n.IsScissorEnabled = this.IsScissorEnabled;
                n.EnableViewFrustumCheck = this.EnableViewFrustumCheck;
                n.PostEffects = this.PostEffects;
            }
            base.AssignDefaultValuesToSceneNode(node);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public sealed class BoundChangedEventArgs : EventArgs
    {
        /// <summary>
        /// The new bound
        /// </summary>
        public readonly BoundingBox NewBound;
        /// <summary>
        /// The old bound
        /// </summary>
        public readonly BoundingBox OldBound;
        /// <summary>
        /// Initializes a new instance of the <see cref="BoundChangedEventArgs"/> class.
        /// </summary>
        /// <param name="newBound">The new bound.</param>
        /// <param name="oldBound">The old bound.</param>
        public BoundChangedEventArgs(BoundingBox newBound, BoundingBox oldBound)
        {
            NewBound = newBound;
            OldBound = oldBound;
        }
    }
}
