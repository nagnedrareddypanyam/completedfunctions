 Esri.ArcGISRuntime.Geometry.PointCollection importedlinepoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
        Esri.ArcGISRuntime.Geometry.PointCollection normalizedimportedpoints= new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);
        private void Import_Click_new(object sender, RoutedEventArgs e)
        {
            RemoveHeilight();
            _sketchOverlay.Graphics.Clear();
            importedlinepoints.Clear();
            routewaypointoverlay.Graphics.Clear();
            try
            {
                string strfilename = "";
                Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
                // openFileDialog.Filter = "All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    openFileDialog.DefaultExt = ".bsk";
                    openFileDialog.Filter = "Basket(.bsk)|*.bsk";
                    strfilename = openFileDialog.FileName;
                    strfilename = openFileDialog.FileName;
                }

                XDocument doc1 = XDocument.Load(strfilename);
                XNamespace ns = "http://www.cirm.org/RTZ/1/0";
                List<MapPoint> _mappoint = new List<MapPoint>();
                // Esri.ArcGISRuntime.Geometry.PointCollection importedpoints = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);
                // IReadOnlyList<MapPoint> _mappoint=null;
                foreach (XElement element in doc1.Root
                                    .Element(ns + "waypoints")
                                    .Elements(ns + "waypoint")
                                    .Elements(ns + "position"))
                {
                    Console.WriteLine("Name: {0}; Value: {1}",
                         (double)element.FirstAttribute,
                         (double)element.LastAttribute);
                    MapPoint mpt = new MapPoint((double)element.LastAttribute, (double)element.FirstAttribute);
                    _mappoint.Add(mpt);
                    importedlinepoints.Add(mpt);
                }
                //SelectPrdctsunderRoot_Click(_mappoint);
                //distancemes();

                normalizedimportedpoints = CalcNormalize_latest(_mappoint);
                var pointline = loadrouteline_create(normalizedimportedpoints);

                var roadPolyline = pointline as Esri.ArcGISRuntime.Geometry.Polyline;
                // var roadPolyline = graphic.Geometry as Esri.ArcGISRuntime.Geometry.Polyline;
                this.polylineBuilder = new PolylineBuilder(roadPolyline);
                if (polylineBuilder.Parts.Count > 1)
                {
                    var item = coordinatesystem_polyline_new(pointline);
                    foreach (var item1 in item)
                    {
                        // var set = Graphiccoordinates_Aftertransform(item1);
                        _sketchOverlay.Graphics.Add(item1);
                        //route_symbolsadding_webmerc(get);
                        //gridrouteline.Add(item1);
                    }
                }
                else
                {
                    var loadedgraphic = coordinatesystem_polyline(pointline);
                    _sketchOverlay.Graphics.Add(loadedgraphic);
                    // route_symbolsadding_webmerc(get);

                }
                route_symbolsadding_webmerc(normalizedimportedpoints);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
		 private Esri.ArcGISRuntime.Geometry.PointCollection CalcNormalize_latest(List<MapPoint> mpt)
        {
            double mptx1 = 0;
            double mptx2 = 0;
            double a = 0;
            int count = 0;
            double mptx2new = 0;

            int index1 = 0;
            int index2 = 0;
            MapPoint mpt2 = null;
            MapPoint mpt1 = null;

            for (int i = 0; i <= mpt.Count; i++)//reading correct values indexed
            {
                if (i + 1 >= mpt.Count)
                {
                    Console.WriteLine("for {0} mappoint value is {1}  ", i, mpt[i--]);
                    break;

                }
                Console.WriteLine("for {0} mappoint value is {1}  ", i, mpt[i]);
            }

            Esri.ArcGISRuntime.Geometry.PointCollection transformed_new = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.WebMercator);

            int j = 0;
            var p = mpt[j].X;

            for (int i = 0; i < mpt.Count; i++)
            {

                var x0 = mpt[i].X;

                if (i+1  == mpt.Count+1)
                {
                    //MapPoint mptx = new MapPoint(mpt[i--].X, mpt[i--].Y, SpatialReferences.Wgs84);
                    //var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                   // transformed_new.Add(aftergr);


                    break;

                }
                if (i == 0)
                {
                    MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                    var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                    transformed_new.Add(aftergr);
                }
                else
                {
                    var x1 = mpt[i - 1].X;
                    var x2 = mpt[i].X;
                    var x3 = x2 - x1;

                    if ((p > 0 && x2 - p > 0))
                    {
                        MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                        var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                        var x4 = ((mpt[i].X - p) + p);
                        var x4new = x4 * 111319.491;
                        MapPoint aftermappoint = new MapPoint(x4new, aftergr.Y, SpatialReference.Create(3857));
                        transformed_new.Add(aftermappoint);
                    }
                    else if ((p > 0 && x2 - p < 0))
                    {
                        MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                        var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                        var x5 = (((mpt[i].X - p) + 360) + p);
                        var x5new = x5 * 111319.491;
                        MapPoint aftermappoint = new MapPoint(x5new, aftergr.Y, SpatialReference.Create(3857));
                        transformed_new.Add(aftermappoint);

                    }
                    else if ((p < 0 && x2 - p > 0))
                    {
                        MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                        var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                        var x6 = ((mpt[i].X - p) + p);
                        var x6new = x6 * 111319.491;
                        MapPoint aftermappoint = new MapPoint(x6new, aftergr.Y, SpatialReference.Create(3857));
                        transformed_new.Add(aftermappoint);
                    }
                    else if ((p < 0 && x2 - p < 0))
                    {
                        MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                        var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                        var x7 = (((mpt[i].X - p) + 360) + p);
                        var x7new = x7 * 111319.491;
                        MapPoint aftermappoint = new MapPoint(x7new, aftergr.Y, SpatialReference.Create(3857));
                        transformed_new.Add(aftermappoint);
                    }
                    else
                    {
                        MapPoint mptx = new MapPoint(mpt[i].X, mpt[i].Y, SpatialReferences.Wgs84);
                        var aftergr = (Esri.ArcGISRuntime.Geometry.MapPoint)GeometryEngine.Project(mptx, SpatialReference.Create(3857));
                        transformed_new.Add(aftergr);
                    }

                }

            }
            return transformed_new;
        }
		 private async void route_symbolsadding_webmerc(Esri.ArcGISRuntime.Geometry.PointCollection PointCollection)
        {

            foreach (var tem in PointCollection)
            {
                polist.polylinepointcollection = null;
                switch (SampleState.AddingStops)
                {
                    case SampleState.AddingStops:
                        // Get the name of this stmop.
                        string stopName = $"W{routewaypointoverlay.Graphics.Count + 1 }";
                        // polist.WaypointCount = _sketchRouteOverlay.Graphics.Count + 1;
                        // Create the marker to show underneath the stop number.
                        PictureMarkerSymbol pushpinMarker = await GetPictureMarker();

                        polist.latitude = tem.X;
                        polist.longitude = tem.Y;


                        TextSymbol stopSymbol = new TextSymbol(stopName, System.Drawing.Color.Transparent, 15,
                            Esri.ArcGISRuntime.Symbology.HorizontalAlignment.Right, Esri.ArcGISRuntime.Symbology.VerticalAlignment.Top);
                        stopSymbol.OffsetY = 0;

                        CompositeSymbol combinedSymbol = new CompositeSymbol(new MarkerSymbol[] { pushpinMarker, stopSymbol });

                        Graphic stopGraphic = new Graphic(tem, combinedSymbol);
                        // Graphic stopGraphic = new Graphic(tem);
                        stopGraphic.Attributes["ShortName"] = stopName;
                        routewaypointoverlay.Graphics.Add(stopGraphic);
                        break;
                }

            }
            // Normalize geometry - important for geometries that will be sent to a server for processing.
            //mapLocation = (MapPoint)GeometryEngine.NormalizeCentralMeridian(mapLocation);
            // var ste = Mapcoordinates_Change(mapLocation);
        }