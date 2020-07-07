 private async void EditButton_Click_new(object sender, RoutedEventArgs e)
        {
            RemoveHeilight();
            _sketchOverlay.Graphics.Clear();
            routewaypointoverlay.Graphics.Clear();
            MyMapView.GeoViewTapped -= MapViewTapped_Mouse_Point;
            savemenu.IsEnabled = true;
            Esri.ArcGISRuntime.Geometry.PointCollection databasepointcollection_web = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
            List<MapPoint> databasepointslist = new List<MapPoint>();

            DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
            DataTable dt = new DataTable();
            dt = objRout.GetRouteLineDetails(SelectedRoutName);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this
                MapPoint mp1 = new MapPoint(latit, longit, SpatialReferences.WebMercator);
                databasepointcollection_web.Add(latit, longit);
                databasepointslist.Add(mp1);
            }


            var l1 = loadrouteline_create(databasepointcollection_web);
            var editPolyline1 = l1 as Polyline;
            this.editlinebuilder = new PolylineBuilder(editPolyline1);
            var item2geom = loadrouteline_geom_create_new(databasepointcollection_web);
            try
            {

                if (editlinebuilder.Parts.Count > 1)
                {
                   
                    var config = new SketchEditConfiguration()
                    {
                        AllowVertexEditing = true,
                        AllowMove = true,
                        AllowRotate = false,

                        ResizeMode = SketchResizeMode.None
                    };
                    sketchEditor();
                    Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(item2geom);
                }
                else
                {
                    var config = new SketchEditConfiguration()
                    {
                        AllowVertexEditing = true,
                        AllowMove = true,
                        AllowRotate = false,

                        ResizeMode = SketchResizeMode.None
                    };
                    sketchEditor();
                    Esri.ArcGISRuntime.Geometry.Geometry newGeometry = await MyMapView.SketchEditor.StartAsync(l1);

                }
            }
            catch (TaskCanceledException)
            {
                // Ignore ... let the user cancel editing
            }
            catch (Exception ex)
            {
                // Report exceptions
                MessageBox.Show("Error editing shape: " + ex.Message);
            }
        }