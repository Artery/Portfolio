using System;
using System.Linq;

//Code snippet from another project
//This command moves items of a ListBox (WPF-App)
//The ListBox has a Collection of Layers (items), a layer can be active or inactive, which indicates if it is visible in the ListBox
//The ZValue determines the position in the ListBox (descending order), 1 ist the lowest item
//The method is generalized to support increasing and decreasing with the same code
//I really liked the way how I generalized the problem to reuse the same code
public class ChangeLayerOrderCommand
{
    //True will lead to increasing, false to decreasing order
    public void Execute(bool increaseLayerOrder)
    {
        //What LayerTool, ClientViewModel and project are exactly isn't important for this example
        var project = m_LayerTool.ClientViewModel.Project;
        var selectedLayer = project.SelectedMapLayer;
        var showInactiveLayers = project.ShowInactiveLayers;
        var mapLayers = project.MapLayers;

        var orderedLayers = increaseLayerOrder ? mapLayers.OrderBy(l => l.ZValue) : mapLayers.OrderByDescending(l => l.ZValue);

        //Delegate to compare two ints
        Func<int, int, bool, bool> compareValues = (lhs, rhs, greater) => greater ? lhs > rhs : lhs < rhs;
        var lastLayerToSwap = orderedLayers.FirstOrDefault(l => compareValues(l.ZValue, selectedLayer.ZValue, increaseLayerOrder)
                                && (l.IsActive || showInactiveLayers));

        //If inactive layers are not visible, they will be "skipped" when moving down the selected layer
        //But when the selected layer is the last visible layer it will not be moved beneath lower inactive layers and vice versa for moving upwards
        /* Example1: 
         * before moving l1 downwards       after moving l1
         * l1 (active, selected layer)      l2 (inactive)
         * l2 (inactive)                    l3 (inactive)
         * l3 (inactive)                    l4 (active)      
         * l4 (active) (lastLayerToSwap)    l1 (active, selected layer) 
         * 
         * Example2:
         * before moving l1 downwards       after moving l1
         * l4 (active)                      l4 (active)
         * l1 (active, selected layer)      l1 (active, selected layer)
         * l2 (inactive)                    l3 (inactive)
         * lastLayerToSwap is null in this case
         */
        //And vice versa for moving upwards
        //If inactive layers visible, they will be treated like active layers

        if (lastLayerToSwap != null || showInactiveLayers)
        {
            var zValues = mapLayers.Select(l => l.ZValue).DefaultIfEmpty(0);
            var fallbackZValueBound = increaseLayerOrder ? zValues.Max() + 1 : zValues.Min() - 1;

            var zValueBound = lastLayerToSwap != null ? lastLayerToSwap.ZValue : fallbackZValueBound;

            var nextInactiveLayers = orderedLayers.Where(l => !l.IsActive
                                        && compareValues(l.ZValue, selectedLayer.ZValue, increaseLayerOrder)
                                        && compareValues(zValueBound, l.ZValue, increaseLayerOrder)).ToList();

            foreach (var layer in nextInactiveLayers)
            {
                if (increaseLayerOrder) { SwapZValues(layer, selectedLayer); }
                else { SwapZValues(selectedLayer, layer); }
            }

            if (lastLayerToSwap != null)
            {
                if (increaseLayerOrder) { SwapZValues(lastLayerToSwap, selectedLayer); }
                else { SwapZValues(selectedLayer, lastLayerToSwap); }
            }
        }

        if (m_LayerTool.ClientViewModel.MapTool.SynchronizeSelectedLayer)
        {
            m_LayerTool.ClientViewModel.Project.UpdateLayers();
        }
    }

    private static void SwapZValues(MapLayerViewModel layer1, MapLayerViewModel layer2)
    {
        int z = layer1.ZValue;
        layer1.ZValue = layer2.ZValue;
        layer2.ZValue = z;
    }
}






