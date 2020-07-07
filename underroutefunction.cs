 PolylineBuilder loadedpolylinebuilder = null;
        private void UnderpolylineRoot_Click(object sender, RoutedEventArgs e)//add this new method and in xaml event  for products under button in routeline tree
        {
            try
            {

                var temproutegeom = MyMapView.SketchEditor.Geometry;//for routeline incomplete draw
                if (temproutegeom != null)
                {
                    Esri.ArcGISRuntime.Geometry.Polyline poly = (Esri.ArcGISRuntime.Geometry.Polyline)GeometryEngine.NormalizeCentralMeridian(temproutegeom);
                    if (poly.Parts.Count > 1)
                    {
                        var tempgraphic = coordinatesystem_polyline_new(poly);
                        foreach (var item in tempgraphic)
                        {
                            var ge = Graphiccoordinates_Aftertransform(item);
                            var graphic = new Graphic(ge);
                            Geometry_OnviewTap(graphic);
                            // Geometry_OnviewTap(item);
                        }
                    }
                    else
                    {
                        var tempgraphic = coordinatesystem_polyline(temproutegeom);
                        Geometry_OnviewTap(tempgraphic);
                    }

                }
                else
                {
                  
                    DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
                    DataTable dt = new DataTable();
                    dt = objRout.GetRouteLineDetails(SelectedRoutName);
                    Esri.ArcGISRuntime.Geometry.PointCollection pointcollection_load = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
                    List<MapPoint> loadpointlist = new List<MapPoint>();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                        var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this
                        MapPoint mp1 = new MapPoint(latit, longit, SpatialReferences.WebMercator);
                        pointcollection_load.Add(latit, longit);
                        loadpointlist.Add(mp1);
                    }

                    var l1 = loadrouteline_create(pointcollection_load);
                    var loadedpolyline = l1 as Polyline;
                    this.loadedpolylinebuilder = new PolylineBuilder(loadedpolyline);
                    var item2geom = loadrouteline_geom_create_new(pointcollection_load);
                    
                    if (loadedpolylinebuilder.Parts.Count > 1)
                    {
                        var tempgraphic = coordinatesystem_polyline_new(l1);
                        foreach (var item in tempgraphic)
                        {
                            var ge = Graphiccoordinates_Aftertransform(item);
                            var graphic = new Graphic(ge);
                            Geometry_OnviewTap(graphic);
                            // Geometry_OnviewTap(item);
                        }
                    }
                    else
                    {
                        var grap=coordinatesystem_polyline(l1);
                        Geometry_OnviewTap(grap);

                    }

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }