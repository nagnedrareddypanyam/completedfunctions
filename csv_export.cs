private void csv_export(object sender, RoutedEventArgs e)
        {
            List<double> distance = new List<double>();
            Esri.ArcGISRuntime.Geometry.PointCollection geo_points = new Esri.ArcGISRuntime.Geometry.PointCollection(SpatialReferences.Wgs84);

            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".csv";
            dlg.Filter = "RTZ(.rtz)|*.rtz | Tokyo Keiki EC8x00|*.csv";
            dlg.ShowDialog();
            string fpath = dlg.FileName;
            string fpath_new = fpath ;
            //string filePath = @"C:\Users\nanip\OneDrive\Desktop\routelineexport\File7.csv";
            DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
            DataTable dt = new DataTable();
            dt = objRout.GetRouteLineDetails(SelectedRoutName);
           
            using (TextWriter sw = new StreamWriter(fpath_new))
            {
                sw.WriteLine("Route Name:,{0}",SelectedRoutName);
                sw.WriteLine(@"Way Point,Position, ,Radius(m),Reach(L),ROT(°/min),XTD(m),SPD(kn),RL/GC,Leg(°),Distance (NM), ,ETA");
                sw.WriteLine(@"ID, LAT,LON, , , , , , , ,To WPT,TOTAL");

               

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                    var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this

                    MapPoint mp = new MapPoint(latit, longit, SpatialReferences.WebMercator);//add this
                    MapPoint aftertrans = (MapPoint)GeometryEngine.Project(mp, SpatialReference.Create(4326));
                   
                    geo_points.Add(aftertrans);
                }
                for(int j = 0; j < geo_points.Count; j++)
                {
                    var saf = CoordinateFormatter.ToLatitudeLongitude(geo_points[j], LatitudeLongitudeFormat.DegreesDecimalMinutes, 3);
                    var cnt = SpaceCount_manipulatestring(saf);
                    var cnt1 = cnt.Item1;
                    var cnt2 = cnt.Item2;
                    var latstr = cnt2.Split(':');
                    var lat = latstr[0].Insert(latstr[0].Length - 1, "'");
                    var longitu = latstr[1].Insert(latstr[1].Length - 1, "'");
                    if (j >= 1)
                    {
                        GeodeticDistanceResult loxodromeMeasureResult = GeometryEngine.DistanceGeodetic(geo_points.ElementAt(j-1), geo_points.ElementAt(j), LinearUnits.NauticalMiles, AngularUnits.Degrees, GeodeticCurveType.Loxodrome);
                        double laxidromeround = Math.Round(loxodromeMeasureResult.Distance, 2);
                        distance.Add(laxidromeround);
                        double totaldist = distance.ElementAt(j - 1) + distance.ElementAt(j);
                        double rounddecim = Math.Round(totaldist, 2);
                        distance[j]=rounddecim;
                        sw.WriteLine(@"{0},{1},{2},,,,,,,,{3},{4}", " ", lat, longitu,laxidromeround,rounddecim);
                       
                    }
                    else
                    {
                        int loxodromeMeasureResult=0;
                        sw.WriteLine(@"{0},{1},{2},,,,,,,,,{3}", " ", lat, longitu,loxodromeMeasureResult);
                        distance.Add(loxodromeMeasureResult);
                    }

                   
                }
                

                //string strData = "Zaara";
                // float floatData = 324.563F;//Note it's a float not string
                // sw.WriteLine("{0},{1}", strData, floatData.ToString("F2"));
            }
            


        }