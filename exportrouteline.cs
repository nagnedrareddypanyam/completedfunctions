private void ExportRoute_Click(object sender, RoutedEventArgs e)
        {
            XmlDeclaration xmldecl;
            XmlDocument doc = new XmlDocument();
            xmldecl = doc.CreateXmlDeclaration("1.0", null, null);

            //Add the new node to the document.
            XmlElement root1 = doc.DocumentElement;
            doc.InsertBefore(xmldecl, root1);


            XmlElement root = doc.CreateElement("route");
            XmlElement id = doc.CreateElement("routeInfo");



            root.SetAttribute("xmlns:xsi", "http://www.w3.org/2001/XMLSchema-instance");
            root.SetAttribute("xmlns:xsd", "http://www.w3.org/2001/XMLSchema");
            root.SetAttribute("version", "1.0");
            root.SetAttribute("xmlns", "http://www.cirm.org/RTZ/1/0");
            root.SetAttribute("xmlns", "http://www.cirm.org/RTZ/1/0");
            id.SetAttribute("routeName", SelectedRoutName);



            root.AppendChild(id);
            XmlElement id1 = doc.CreateElement("defaultWaypoint");
            id1.SetAttribute("radius", "0.5");
            XmlElement ws = doc.CreateElement("leg");
            ws.SetAttribute("starboardXTD", "0.1");
            ws.SetAttribute("portsideXTD", "0.1");
            id1.AppendChild(ws);

            XmlElement id2 = doc.CreateElement("waypoints");
            id2.AppendChild(id1);

            DataAccessLayer.RoutRepository objRout = new DataAccessLayer.RoutRepository();
            DataTable dt = new DataTable();
            dt = objRout.GetRouteLineDetails(SelectedRoutName);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                XmlElement wsy = doc.CreateElement("waypoint");
                XmlElement wsy1 = doc.CreateElement("position");

                XmlElement wsy2 = doc.CreateElement("leg");
                int j = i + 1;
                wsy.SetAttribute("id", j.ToString());
                wsy.SetAttribute("revision", "1");

                var latit = Convert.ToDouble(dt.Rows[i]["Latitude"]);//add this
                var longit = Convert.ToDouble(dt.Rows[i]["Longitude"]);//add this
                MapPoint mp = new MapPoint(latit, longit, SpatialReferences.WebMercator);//add this


                MapPoint aftertrans = (MapPoint)GeometryEngine.Project(mp, SpatialReference.Create(4326));//add this


                wsy1.SetAttribute("lat", aftertrans.Y.ToString());//add this
                wsy1.SetAttribute("lon", aftertrans.X.ToString());//add this

                // wsy1.SetAttribute("lat", dt.Rows[i]["Latitude"].ToString());//comment this
                // wsy1.SetAttribute("lon", dt.Rows[i]["Longitude"].ToString());//comment this

                wsy.AppendChild(wsy1);
                wsy.AppendChild(wsy2);

                id2.AppendChild(wsy);
                root.AppendChild(id2);
            }

            doc.AppendChild(root);
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.DefaultExt = ".rtz";
            dlg.Filter = "RTZ(.rtz)|*.rtz";
            if (dlg.ShowDialog() == true)
            {
                doc.Save(dlg.FileName);
            }

        }