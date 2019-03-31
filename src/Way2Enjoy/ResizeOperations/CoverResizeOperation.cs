using System;

namespace Way2enjoy.ResizeOperations
{
    public class CoverResizeOperation : ResizeOperation
    {
        public CoverResizeOperation(int width, int height) : base(ResizeType.Cover, width, height)
        {
            if (width == 0)
            {
                throw new ArgumentException("You must specify a width", nameof(width));
            }
            if (height == 0)
            {
                throw new ArgumentException("You must specify a height", nameof(width));
            }
        }
    }
}
