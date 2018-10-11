using Box2DX.Collision;

namespace Box2DX.Dynamics
{
    public class ContactFilter
    {
        public virtual bool ShouldCollide(Shape shape1, Shape shape2)
        {
            FilterData filterData1 = shape1.FilterData;
            FilterData filterData2 = shape2.FilterData;
            if ((int) filterData1.GroupIndex == (int) filterData2.GroupIndex && filterData1.GroupIndex != (short) 0)
                return filterData1.GroupIndex > (short) 0;
            return ((int) filterData1.MaskBits & (int) filterData2.CategoryBits) != 0 && ((int) filterData1.CategoryBits & (int) filterData2.MaskBits) != 0;
        }

        public bool RayCollide(object userData, Shape shape)
        {
            if (userData == null)
                return true;
            return this.ShouldCollide((Shape) userData, shape);
        }
    }
}