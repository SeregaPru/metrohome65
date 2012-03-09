using System;
using Fleux.UIElements;

internal class TilesCanvas : Canvas
{
    // Methods
    public override void AddElement(UIElement element)
    {
        base.AddElement(element);
        element.Updated = BufferedUpdate;
    }

    public override void AddElementAt(int index, UIElement element)
    {
        base.AddElementAt(index, element);
        element.Updated = BufferedUpdate;
    }

    public void BufferedUpdate()
    {
        if (!FreezeUpdate)
        {
            Update();
        }
    }

    public void DeleteElement(UIElement element)
    {
        element.Parent = null;
        element.Updated = null;
        Children.Remove(element);
    }

    // Properties
    public bool FreezeUpdate { get; set; }
}

