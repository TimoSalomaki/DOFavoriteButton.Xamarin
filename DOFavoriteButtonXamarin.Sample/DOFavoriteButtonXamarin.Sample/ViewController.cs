using Foundation;
using System;
using UIKit;

namespace DOFavoriteButtonXamarin.Sample
{
    public partial class ViewController : UIViewController
    {
        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            ImageButton.AddTarget(ImageButtonTapped, UIControlEvent.TouchUpInside);
            ImageButton2.AddTarget(ImageButtonTapped, UIControlEvent.TouchUpInside);
            ImageButton3.AddTarget(ImageButtonTapped, UIControlEvent.TouchUpInside);
            ImageButton4.AddTarget(ImageButtonTapped, UIControlEvent.TouchUpInside);
            ImageButton5.AddTarget(ImageButtonTapped, UIControlEvent.TouchUpInside);
        }

        private void ImageButtonTapped(object sender, EventArgs e)
        {
            var senderButton = sender as DOFavoriteButton;

            if(senderButton.IsSelected)
            {
                senderButton.Deselect();
            }

            else
            {
                senderButton.Select();
            }
        }

        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}