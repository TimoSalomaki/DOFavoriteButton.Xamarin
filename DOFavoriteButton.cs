using CoreAnimation;
using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;

namespace DOFavoriteButtonXamarin
{
    [Register("DOFavoriteButton"), DesignTimeVisible(true)]
    public class DOFavoriteButton : UIButton
    {
        private UIImage _image;

        private UIColor _imageColorOn = new UIColor(255, 172, 51, 1);
        private UIColor _imageColorOff = new UIColor(136, 153, 166, 1);
        private UIColor _circleColor = new UIColor(155, 172, 51, 1);
        private UIColor _lineColor = new UIColor(250, 120, 68, 1);

        private CAShapeLayer _imageShape;
        private CAShapeLayer _circleShape;
        private CAShapeLayer _circleMask;
        private CAShapeLayer[] _lines;

        private double _duration = 1.0;

        private bool _isSelected;

        private CAKeyFrameAnimation _circleTransform = new CAKeyFrameAnimation() { KeyPath = "transform" };
        private CAKeyFrameAnimation _circleMaskTransform = new CAKeyFrameAnimation() { KeyPath = "transform" };
        private CAKeyFrameAnimation _lineStrokeStart = new CAKeyFrameAnimation() { KeyPath = "strokeStart" };
        private CAKeyFrameAnimation _lineStrokeEnd = new CAKeyFrameAnimation() { KeyPath = "strokeEnd" };
        private CAKeyFrameAnimation _lineOpacity = new CAKeyFrameAnimation() { KeyPath = "opacity" };
        private CAKeyFrameAnimation _imageTransform = new CAKeyFrameAnimation() { KeyPath = "transform" };

        public DOFavoriteButton(CGRect frame) : this(frame, new UIImage()) { }
        public DOFavoriteButton() : this(CGRect.Empty) { }

        public DOFavoriteButton(CGRect frame, UIImage image) : base(frame)
        {
            _image = image;
            CreateLayers();
            AddTargets();
        }

        public DOFavoriteButton(NSCoder aDecoder) : base(aDecoder)
        {
            CreateLayers();
            AddTargets();
        }

        public UIImage Image
        {
            get
            {
                return _image;
            }
            set
            {
                _image = value;
                CreateLayers();
            }
        }

        public UIColor ImageColorOn
        {
            get
            {
                return _imageColorOn;
            }

            set
            {
                _imageColorOn = value;

                if (IsSelected)
                {
                    _imageShape.FillColor = _imageColorOn.CGColor;
                }
            }
        }

        public UIColor ImageColorOff
        {
            get
            {
                return _imageColorOff;
            }

            set
            {
                _imageColorOff = value;

                if (!IsSelected)
                {
                    _imageShape.FillColor = _imageColorOff.CGColor;
                }
            }
        }

        public UIColor CircleColor
        {
            get
            {
                return _circleColor;
            }

            set
            {
                _circleColor = value;
                _circleShape.FillColor = _circleColor.CGColor;
            }
        }

        public UIColor LineColor
        {
            get
            {
                return _lineColor;
            }

            set
            {
                _lineColor = value;
                foreach (var line in _lines)
                {
                    _lineColor = value;
                    line.StrokeColor = _lineColor.CGColor;
                }
            }
        }

        public double Duration
        {
            get
            {
                return _duration;
            }

            set
            {
                _duration = value;
                _circleTransform.Duration = 0.333 * _duration;
                _circleMaskTransform.Duration = 0.333 * _duration;
                _lineStrokeStart.Duration = 0.6 * _duration;
                _lineStrokeEnd.Duration = 0.6 * _duration;
                _lineOpacity.Duration = 1.0 * _duration;
                _imageTransform.Duration = 1.0 * _duration;
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }

            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    if (_isSelected)
                    {
                        _imageShape.FillColor = _imageColorOn.CGColor;
                    }

                    else
                    {
                        Deselect();
                    }
                }
            }
        }

        private void CreateLayers()
        {
            this.Layer.Sublayers = null;

            var imageFrame = new CGRect(Frame.Size.Width / 2 - Frame.Size.Width / 4, Frame.Size.Height / 2 - Frame.Size.Height / 4, Frame.Size.Width / 2, Frame.Size.Height / 2);
            var imgCenterPoint = new CGPoint(imageFrame.GetMidX(), y: imageFrame.GetMidY());
            var lineFrame = new CGRect(imageFrame.X - imageFrame.Width / 4, imageFrame.Y - imageFrame.Height / 4, imageFrame.Width * 1.5, imageFrame.Height * 1.5);

            CreateCircleShape(imageFrame, imgCenterPoint);
            CreateLines(lineFrame, imgCenterPoint);
            CreateImage(imageFrame, imgCenterPoint);
            CreateCircleTransformAnimation(imageFrame);
            CreateLineStrokeAnimation();
            CreateImageTransformAnimation();
        }

        private void CreateCircleShape(CGRect imageFrame, CGPoint imgCenterPoint)
        {
            _circleShape = new CAShapeLayer()
            {
                Bounds = imageFrame,
                Position = imgCenterPoint,
                Path = UIBezierPath.FromRect(imageFrame).CGPath,
                FillColor = _circleColor.CGColor,
                Transform = CATransform3D.MakeScale(0.0F, 0.0F, 1.0F)
            };

            this.Layer.AddSublayer(_circleShape);

            _circleMask = new CAShapeLayer()
            {
                Bounds = imageFrame,
                Position = imgCenterPoint,
                FillRule = CAShapeLayer.FillRuleEvenOdd
            };

            _circleShape.Mask = _circleMask;

            var maskPath = UIBezierPath.FromRect(imageFrame);
            maskPath.AddArc(imgCenterPoint, 0.1F, 0.0F, (System.nfloat)(Math.PI * 2F), true);
            _circleMask.Path = maskPath.CGPath;
        }

        private void CreateLines(CGRect lineFrame, CGPoint imgCenterPoint)
        {
            var path = new CGPath();
            path.MoveToPoint(new CGPoint(lineFrame.GetMidX(), lineFrame.GetMidY()));
            path.AddLineToPoint(new CGPoint(lineFrame.X + lineFrame.Width / 2, lineFrame.Y));

            _lines = new CAShapeLayer[5];

            for (int i = 0; i < 5; i++)
            {
                var line = new CAShapeLayer()
                {
                    Bounds = lineFrame,
                    Position = imgCenterPoint,
                    MasksToBounds = true,
                    Actions = new NSDictionary("strokeStart", new NSNull(), "strokeEnd", new NSNull()),
                    StrokeColor = _lineColor.CGColor,
                    LineWidth = 1.25F,
                    MiterLimit = 1.25F,
                    Path = path,
                    LineCap = CAShapeLayer.CapRound,
                    LineJoin = CAShapeLayer.JoinRound,
                    StrokeStart = 0.0F,
                    StrokeEnd = 0.0F,
                    Opacity = 0.0F,
                    Transform = CATransform3D.MakeRotation((nfloat)Math.PI / 5F * (nfloat)(i * 2F + 1), 0.0F, 0.0F, 1.0F)
                };

                this.Layer.AddSublayer(line);
                _lines[i] = line;
            }
        }

        private void CreateImage(CGRect imageFrame, CGPoint imgCenterPoint)
        {
            var mask = new CALayer()
            {
                Contents = _image.CGImage,
                Bounds = imageFrame,
                Position = imgCenterPoint
            };

            _imageShape = new CAShapeLayer()
            {
                Bounds = imageFrame,
                Position = imgCenterPoint,
                Path = UIBezierPath.FromRect(imageFrame).CGPath,
                FillColor = _imageColorOff.CGColor,
                Actions = new NSDictionary("fillColor", new NSNull()),
                Mask = mask
            };

            this.Layer.AddSublayer(_imageShape);
        }

        private void CreateCircleTransformAnimation(CGRect imageFrame)
        {
            _circleTransform.Duration = 0.333; // 0.0333 * 10
            _circleTransform.Values = new NSObject[8] {
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.0F, 0.0F, 1.0F)),    //  0/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.5F, 0.5F, 1.0F)),    //  1/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.0F, 1.0F, 1.0F)),    //  2/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.2F, 1.0F, 1.0F)),    //  3/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.3F, 1.3F, 1.0F)),    //  4/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.37F, 1.37F, 1.0F)),  //  5/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.4F, 1.4F, 1.0F)),    //  6/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.4F, 1.4F, 1.0F))     // 10/10
                };
            _circleTransform.KeyTimes = new NSNumber[8] {
                    NSNumber.FromDouble(0.0),    //  0/10
                    NSNumber.FromDouble(0.1),    //  1/10
                    NSNumber.FromDouble(0.2),    //  2/10
                    NSNumber.FromDouble(0.3),    //  3/10
                    NSNumber.FromDouble(0.4),    //  4/10
                    NSNumber.FromDouble(0.5),    //  5/10
                    NSNumber.FromDouble(0.6),    //  6/10
                    NSNumber.FromDouble(1.0)     // 10/10
                };

            _circleMaskTransform.Duration = 0.333; // 0.0333 * 10
            _circleMaskTransform.Values = new NSObject[9] {
                    NSValue.FromCATransform3D(CATransform3D.Identity),                                                                  //  0/10
                    NSValue.FromCATransform3D(CATransform3D.Identity),                                                                  //  2/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(imageFrame.Width * 1.25F, imageFrame.Height * 1.25F, 1.0F)),      //  3/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(imageFrame.Width * 2.688F, imageFrame.Height * 2.688F, 1.0F)),    //  4/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(imageFrame.Width * 3.923F, imageFrame.Height * 3.923F, 1.0F)),    //  5/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(imageFrame.Width * 4.375F, imageFrame.Height * 4.375F, 1.0F)),    //  6/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(imageFrame.Width * 4.731F, imageFrame.Height * 4.731F, 1.0F)),    //  7/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(imageFrame.Width * 5.0F, imageFrame.Height * 5.0F, 1.0F)),        //  9/10
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(imageFrame.Width * 5.0F, imageFrame.Height * 5.0F, 1.0F))         // 10/10
                };
            _circleMaskTransform.KeyTimes = new NSNumber[9] {
                    NSNumber.FromDouble(0.0),    //  0/10
                    NSNumber.FromDouble(0.2),    //  2/10
                    NSNumber.FromDouble(0.3),    //  3/10
                    NSNumber.FromDouble(0.4),    //  4/10
                    NSNumber.FromDouble(0.5),    //  5/10
                    NSNumber.FromDouble(0.6),    //  6/10
                    NSNumber.FromDouble(0.7),    //  7/10
                    NSNumber.FromDouble(0.9),    //  9/10
                    NSNumber.FromDouble(1.0)     // 10/10
                };
        }

        private void CreateLineStrokeAnimation()
        {
            _lineStrokeStart.Duration = 0.6; //0.0333 * 18
            _lineStrokeStart.Values = new NSObject[11] {
                    NSNumber.FromDouble(0.0),    //  0/18
                    NSNumber.FromDouble(0.0),    //  1/18
                    NSNumber.FromDouble(0.18),   //  2/18
                    NSNumber.FromDouble(0.2),    //  3/18
                    NSNumber.FromDouble(0.26),   //  4/18
                    NSNumber.FromDouble(0.32),   //  5/18
                    NSNumber.FromDouble(0.4),    //  6/18
                    NSNumber.FromDouble(0.6),    //  7/18
                    NSNumber.FromDouble(0.71),   //  8/18
                    NSNumber.FromDouble(0.89),   // 17/18
                    NSNumber.FromDouble(0.92)    // 18/18
                };
            _lineStrokeStart.KeyTimes = new NSNumber[11] {
                NSNumber.FromDouble(0.0),    //  0/18
                NSNumber.FromDouble(0.056),  //  1/18
                NSNumber.FromDouble(0.111),  //  2/18
                NSNumber.FromDouble(0.167),  //  3/18
                NSNumber.FromDouble(0.222),  //  4/18
                NSNumber.FromDouble(0.278),  //  5/18
                NSNumber.FromDouble(0.333),  //  6/18
                NSNumber.FromDouble(0.389),  //  7/18
                NSNumber.FromDouble(0.444),  //  8/18
                NSNumber.FromDouble(0.944),  // 17/18
                NSNumber.FromDouble(1.0)     // 18/18
            };

            _lineStrokeEnd.Duration = 0.6; //0.0333 * 18
            _lineStrokeEnd.Values = new NSObject[8] {
                    NSNumber.FromDouble(0.0),    //  0/18
                    NSNumber.FromDouble(0.0),    //  1/18
                    NSNumber.FromDouble(0.32),   //  2/18
                    NSNumber.FromDouble(0.48),   //  3/18
                    NSNumber.FromDouble(0.64),   //  4/18
                    NSNumber.FromDouble(0.68),   //  5/18
                    NSNumber.FromDouble(0.92),   // 17/18
                    NSNumber.FromDouble(0.92)    // 18/18
                };
            _lineStrokeEnd.KeyTimes = new NSNumber[8] {
                    NSNumber.FromDouble(0.0),    //  0/18
                    NSNumber.FromDouble(0.056),  //  1/18
                    NSNumber.FromDouble(0.111),  //  2/18
                    NSNumber.FromDouble(0.167),  //  3/18
                    NSNumber.FromDouble(0.222),  //  4/18
                    NSNumber.FromDouble(0.278),  //  5/18
                    NSNumber.FromDouble(0.944),  // 17/18
                    NSNumber.FromDouble(1.0),    // 18/18
                };

            _lineOpacity.Duration = 1.0; //0.0333 * 30
            _lineOpacity.Values = new NSObject[3] {
                    NSNumber.FromDouble(1.0),    //  0/30
                    NSNumber.FromDouble(1.0),    // 12/30
                    NSNumber.FromDouble(0.0)     // 17/30
                };
            _lineOpacity.KeyTimes = new NSNumber[3] {
                    NSNumber.FromDouble(0.0),    //  0/30
                    NSNumber.FromDouble(0.4),    // 12/30
                    NSNumber.FromDouble(0.567)   // 17/30
                };
        }

        private void CreateImageTransformAnimation()
        {
            _imageTransform.Duration = 1.0; //0.0333 * 30
            _imageTransform.Values = new NSObject[17] {
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.0F, 0.0F, 1.0F)),       //  0/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.0F, 0.0F, 1.0F)),       //  3/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.2F, 1.2F, 1.0F)),       //  9/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.25F, 1.25F, 1.0F)),     // 10/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.2F, 1.2F, 1.0F)),       // 11/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.9F, 0.9F, 1.0F)),       // 14/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.875F, 0.875F, 1.0F)),   // 15/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.875F, 0.875F, 1.0F)),   // 16/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.9F, 0.9F, 1.0F)),       // 17/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.013F, 1.013F, 1.0F)),   // 20/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.025F, 1.025F, 1.0F)),   // 21/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(1.013F, 1.013F, 1.0F)),   // 22/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.96F, 0.96F, 1.0F)),     // 25/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.95F, 0.95F, 1.0F)),     // 26/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.96F, 0.96F, 1.0F)),     // 27/30
                    NSValue.FromCATransform3D(CATransform3D.MakeScale(0.99F, 0.99F, 1.0F)),     // 29/30
                    NSValue.FromCATransform3D(CATransform3D.Identity)                           // 30/30
                    };
            _imageTransform.KeyTimes = new NSNumber[17] {
                    NSNumber.FromDouble(0.0),    //  0/30
                    NSNumber.FromDouble(0.1),    //  3/30
                    NSNumber.FromDouble(0.3),    //  9/30
                    NSNumber.FromDouble(0.333),  // 10/30
                    NSNumber.FromDouble(0.367),  // 11/30
                    NSNumber.FromDouble(0.467),  // 14/30
                    NSNumber.FromDouble(0.5),    // 15/30
                    NSNumber.FromDouble(0.533),  // 16/30
                    NSNumber.FromDouble(0.567),  // 17/30
                    NSNumber.FromDouble(0.667),  // 20/30
                    NSNumber.FromDouble(0.7),    // 21/30
                    NSNumber.FromDouble(0.733),  // 22/30
                    NSNumber.FromDouble(0.833),  // 25/30
                    NSNumber.FromDouble(0.867),  // 26/30
                    NSNumber.FromDouble(0.9),    // 27/30
                    NSNumber.FromDouble(0.967),  // 29/30
                    NSNumber.FromDouble(1.0)     // 30/30
                };
        }

        private void AddTargets()
        {
            AddTarget(Handle_TouchDown, UIControlEvent.TouchDown);
            AddTarget(Handle_TouchUpInside, UIControlEvent.TouchUpInside);
            AddTarget(Handle_TouchDragExit, UIControlEvent.TouchDragExit);
            AddTarget(Handle_TouchDragEnter, UIControlEvent.TouchDragEnter);
            AddTarget(Handle_TouchCancel, UIControlEvent.TouchCancel);
        }

        void Handle_TouchDown(object sender, EventArgs e)
        {
            this.Layer.Opacity = 0.4F;
        }

        void Handle_TouchUpInside(object sender, EventArgs e)
        {
            this.Layer.Opacity = 1F;
        }

        void Handle_TouchDragExit(object sender, EventArgs e)
        {
            this.Layer.Opacity = 1F;
        }

        void Handle_TouchDragEnter(object sender, EventArgs e)
        {
            this.Layer.Opacity = 0.4F;
        }

        void Handle_TouchCancel(object sender, EventArgs e)
        {
            this.Layer.Opacity = 1F;
        }

        void Select()
        {
            _isSelected = true;
            _imageShape.FillColor = _imageColorOn.CGColor;

            CATransaction.Begin();

            _circleShape.AddAnimation(_circleTransform, "transform");
            _circleMask.AddAnimation(_circleMaskTransform, "transform");
            _imageShape.AddAnimation(_imageTransform, "transform");

            for (int i = 0; i < 5; i++)
            {
                _lines[i].AddAnimation(_lineStrokeStart, "strokeStart");
                _lines[i].AddAnimation(_lineStrokeEnd, "strokeEnd");
                _lines[i].AddAnimation(_lineOpacity, "opacity");
            }

            CATransaction.Commit();
        }

        void Deselect()
        {
            _isSelected = false;
            _imageShape.FillColor = _imageColorOff.CGColor;

            _circleShape.RemoveAllAnimations();
            _circleMask.RemoveAllAnimations();
            _imageShape.RemoveAllAnimations();
            _lines[0].RemoveAllAnimations();
            _lines[1].RemoveAllAnimations();
            _lines[2].RemoveAllAnimations();
            _lines[3].RemoveAllAnimations();
            _lines[4].RemoveAllAnimations();
        }
    }
}