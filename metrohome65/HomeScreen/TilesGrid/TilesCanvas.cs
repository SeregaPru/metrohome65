using Fleux.UIElements;

namespace MetroHome65.HomeScreen.TilesGrid
{
    public class TilesCanvas : 
        FreezedCanvas  
        //BufferedCanvas
    {

        public void DeleteElement(UIElement element)
        {
            element.Parent = null;
            element.Updated = null;
            Children.Remove(element);
        }

    }
}
