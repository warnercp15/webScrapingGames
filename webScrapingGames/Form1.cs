using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Globalization;

namespace webScrapingGames
{
    public partial class Form1 : Form
    {
        List<string> games = new List<string>() {"uncharted the nathan drake collection","final fantasy vii remake","sekiro shadows die twice","devil may cry 5","the evil within 2","borderlands 3", "cuphead","doom eternal", "fallout 76","fifa 21","payday 2 crimewave edition", "middle earth shadow of mordor","ark survival evolved", "red dead redemption 2","ghost of tsushima","tomb raider definitive edition","need for speed payback","resident evil 0 hd","lego marvel super heroes","batman arkham knight","lego marvel avengers", "lego harry potter collection","street fighter v", "until dawn", "bloodborne", "heavy rain", "dying light","dishonored 2", "just cause 4", "ufc 3","doom", "lego batman 3", "resident evil 5","resident evil hd","mad max", "dirt 4", "tekken 7","lego worlds","mortal kombat x", "battlefield 4"};
        List<ListViewItem> listaItems=new List<ListViewItem>();
        Stopwatch stopWatch = new Stopwatch();
        Stopwatch stopWatch2 = new Stopwatch();

        string pathImages = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + @"/imgs/";

        public Form1()
        {
            InitializeComponent();
            this.listView1.View = View.LargeIcon;
            this.imageList2.ImageSize = new Size(105, 130);
            this.listView1.LargeImageList = this.imageList2;
            label2.ForeColor = Color.White;
            label6.ForeColor = Color.White;
            label8.ForeColor = Color.White;
        }

        private void limpiarCarpeta(DirectoryInfo directoryInfo)
        {
            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                file.Delete();
            }
            Console.WriteLine("\nCarpeta limpiada correctamente.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listView1.Clear();
            this.listView1.View = View.LargeIcon;
            this.imageList2.ImageSize = new Size(105, 130);
            this.listView1.LargeImageList = this.imageList2;
            label2.Text = "";
            label6.Text = "";
            label8.Text = "";
            listaItems = new List<ListViewItem>();
            limpiarCarpeta(new DirectoryInfo(pathImages));
            //limpiar todo

            Console.WriteLine("Inicio de Ejecucion Secuencial.");
            stopWatch.Start();

            getImages();
            Console.WriteLine("Imagenes descargadas correctamente.");

            stopWatch2.Start();
            List<string> listaPrecios = getPreciosSecuencial();
            Console.WriteLine("Precios cargados correctamente.");
            label2.Text = stopWatch2.Elapsed.TotalSeconds + "s";
            stopWatch2.Reset();

            stopWatch2.Start();
            List<string> listaScores = getMetacriticSecuencial();
            Console.WriteLine("Puntajes cargados correctamente.");
            label6.Text = stopWatch2.Elapsed.TotalSeconds + "s";
            stopWatch2.Reset();

            stopWatch2.Start();
            List<Tuple<string, string>> listaTiempos = getHowlogToBeatSecuencial();
            Console.WriteLine("Tiempos cargados correctamente.");
            label8.Text = stopWatch2.Elapsed.TotalSeconds + "s";
            stopWatch2.Reset();

            for (int i = 0; i < games.Count; i++)
            {
                if (listaPrecios[i].Split(';')[1].Length > 12)
                {
                    listaItems[i].BackColor = Color.Yellow;
                }
                listaItems[i].Text +="\n"+ listaPrecios[i].Split(';')[1]+"\n"+ " P:" + listaScores[i].Split(';')[1] + " - HLTB:" + listaTiempos[i].Item2+"h";
                this.listView1.Items.Add(listaItems[i]);
            }
            Console.WriteLine("Datos ordenados correctamente.");
            Console.WriteLine("Tiempo Total: {0} segundos", stopWatch.Elapsed.TotalSeconds);
            Console.WriteLine("Fin de Ejecucion Secuencial.");
            stopWatch.Reset();
            stopWatch2.Reset();
        }

        List<Tuple<string, string>> getHowlogToBeatSecuencial()
        {
            HowLongToBeatPy howLongToBeat = new HowLongToBeatPy(games);      // Instancia de la clase que realiza las consultas
            // Función principal de hltb retorna una lista de tuplas L = [(nombrejuego_1, tiempo_1),...,(nombrejuego_n, tiempo1)]
            List<Tuple<string, string>> hltbResponse = howLongToBeat.GetTimeToBeat();
            return hltbResponse;
        }

        List<string> getMetacriticSecuencial()
        {
            HtmlWeb oWeb = new HtmlWeb();
            string score, url;
            HtmlAgilityPack.HtmlDocument doc;
            List<string> listaScores = new List<string>();
            foreach (var nombre in games)
            {
                url = "https://www.metacritic.com/search/all/" + nombre + "/results";
                doc = oWeb.Load(url);

                score = "";
                foreach (var Nodo in doc.DocumentNode.CssSelect("span.metascore_w.medium.game"))
                {
                    if (score == "")
                    {
                        if (Nodo.InnerText != null && Nodo.InnerText != "tbd")
                        {
                            score = nombre.Replace(" ", "").ToLower() + ";" + float.Parse("" + (10.0 * Int16.Parse(Nodo.InnerText)) / 100.0);
                            listaScores.Add(score);
                            break;
                        }
                    }
                }
                if (score == "")
                {
                    score = nombre.Replace(" ", "").ToLower() + ";" + "n/a";
                    listaScores.Add(score);
                }
            }
            return listaScores;
        }

        void getImages()
        {
            HtmlWeb oWeb = new HtmlWeb();
            WebClient oClient = new WebClient();
            HtmlAgilityPack.HtmlDocument doc;
            string url,img;
            int index = 0;
            foreach (var nombre in games)
            {
                url = "http://dixgamer.com/shop/juegos/ps4/accion-ps4/" + nombre.Replace(" ", "-") + "/?v=1d7b33fc26ca";
                doc = oWeb.Load(url);
                foreach (var Nodo in doc.DocumentNode.CssSelect("div.woocommerce-product-gallery__image"))
                {
                    img = Nodo.GetAttributeValue("data-thumb");
                    oClient.DownloadFile(new Uri(img), pathImages + index + ".jpg");
                    using (Image myImage = Image.FromFile(pathImages + index + ".jpg"))
                    {
                        this.imageList2.Images.Add(myImage);
                        ListViewItem item = new ListViewItem();
                        item.Text = nombre;
                        item.ImageIndex = index;
                        listaItems.Add(item);
                        index++;
                        break;
                    }
                }
            }
        }

        float getIkuroGamesSecuencial(string nombre)
        {
            float price = 0;
            string aux;
            HtmlWeb oWeb = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc;
            string url = "https://ikurogames.com/?s=" + nombre.Replace(" ", "+").ToLower() + "&post_type=product";
            //Console.WriteLine("Buscando: " + nombre+" en IrukoGames");
            doc = oWeb.Load(url);
            //Console.WriteLine(url);
            foreach (var Nodo in doc.DocumentNode.CssSelect("span.price"))
            {
                byte[] bytes = Encoding.ASCII.GetBytes(Nodo.InnerText);
                byte[] byPrecio = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, bytes);
                string precios = Encoding.UTF8.GetString(byPrecio, 0, byPrecio.Length).Replace("&nbsp;", "").Replace("&ndash;", "-").Replace("USD", "").Replace(" ", "");
                if (precios.Length > 10)
                {
                    price = float.Parse(precios.Split('-')[0].Split(';')[1].Replace(".", ""), CultureInfo.InvariantCulture.NumberFormat);
                }
                else if (precios.Length ==8)
                {
                    aux = precios.Split(';')[1];
                    price = float.Parse(aux);
                }
                else
                {
                    price = float.Parse(precios.Split(';')[1].Replace(".", ""), CultureInfo.InvariantCulture.NumberFormat);
                }
                break;
            }
            price = price/float.Parse("79,0307"); //se convierte a dolares porque viene en ARS (pesos argentinos)
            aux = price.ToString("####0.00");
            return float.Parse(aux);
        }
        float getDixGamerGamesSecuencial(string nombre)
        {
            float price = 0;
            Boolean oferta = false;
            string resultado,auxPrice;
            HtmlWeb oWeb = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc;
            //Console.WriteLine("Buscando: " + nombre + " en DixGamer");
            string url = "http://dixgamer.com/shop/juegos/ps4/accion-ps4/" + nombre.Replace(" ", "-") + "/?v=1d7b33fc26ca";
            doc= oWeb.Load(url);
            //Console.WriteLine(url);
            foreach (var Nodo in doc.DocumentNode.CssSelect("p.price"))
            {
                foreach (var Nodo2 in doc.DocumentNode.CssSelect("div.product-images span.onsale"))
                {
                    oferta = true;
                    break;
                }
                byte[] bytes = Encoding.ASCII.GetBytes(Nodo.InnerText);
                byte[] byPrecio = Encoding.Convert(Encoding.ASCII, Encoding.UTF8, bytes);
                string[] precios = Encoding.UTF8.GetString(byPrecio, 0, byPrecio.Length).Replace("&nbsp;", "").Replace("&ndash;", "-").Replace("USD", "").Replace(" ", "").Split('-');
                resultado = precios[0].Split('\n')[1];
                if (resultado.Length>5)
                {
                    auxPrice = ""+resultado[4] + resultado[5] + resultado[6]+ resultado[7];
                    price = float.Parse(auxPrice);
                }
                else
                {
                    price = float.Parse(resultado);
                }
                break;
            }
            if (oferta)
            {
                price = price * -1;
            }
            return price;
        }

        List<string> getPreciosMutiproceso()
        {
            string precio;
            List<string> listaPrecios = new List<string>();

            List<float> listaN1 = new List<float>(games.Count);
            for (int i = 0; i < games.Count; i++) listaN1.Add(0);

            List<float> listaN2 = new List<float>(games.Count);
            for (int i = 0; i < games.Count; i++) listaN2.Add(0);

            Parallel.Invoke(() =>
            {

                Parallel.Invoke(() =>
                {
                    for (int i = 0; i < 20; i++)
                    {
                        listaN1[i] = getDixGamerGamesSecuencial(games[i]);
                    }

                }, () =>
                {
                    for (int i = 20; i < 40; i++)
                    {
                        listaN1[i] = getDixGamerGamesSecuencial(games[i]);
                    }
                });
                Console.WriteLine("Precios de DixGamer cargados correctamente.");


            }, () =>
            {

                Parallel.Invoke(() =>
                {
                    for (int i = 0; i < 20; i++)
                    {
                        listaN2[i] = getIkuroGamesSecuencial(games[i]);
                    }

                }, () =>
                {
                    for (int i = 20; i < 40; i++)
                    {
                        listaN2[i] = getIkuroGamesSecuencial(games[i]);
                    }
                });
                Console.WriteLine("Precios de IkuroGames cargados correctamente.");


            });

            for (int i=0;i<games.Count;i++)
            {
                if (listaN1[i] < 0) //hay oferta
                {
                    if ((listaN1[i] * -1) < listaN2[i])
                    {
                        precio = "oferta: $" + (listaN1[i] * -1) + "-$" + listaN2[i];
                    }
                    else if ((listaN1[i] * -1) == listaN2[i])
                    {
                        precio = "oferta: $" + listaN2[i]; //ponemos solo uno porque son iguales
                    }
                    else
                    {
                        precio = "oferta: $" + listaN2[i] + "-$" + (listaN1[i] * -1);
                    }
                }
                else //no hay oferta
                {
                    if (listaN1[i] < listaN2[i])
                    {
                        precio = "$" + listaN1[i] + "-$" + listaN2[i];
                    }
                    else if (listaN1[i] == listaN2[i])
                    {
                        precio = "$" + listaN2[i];  //ponemos solo uno porque son iguales
                    }
                    else
                    {
                        precio = "$" + listaN2[i] + "-$" + listaN1[i];
                    }
                }
                precio = games[i].Replace(" ", "").ToLower() + ";" + precio;
                listaPrecios.Add(precio);
            }
            return listaPrecios;
        }

        List<string> getPreciosSecuencial()
        {
            float n1, n2;
            string precio;
            List<string> listaPrecios = new List<string>();
            foreach (var game in games)
            {
                n1 = getDixGamerGamesSecuencial(game);
                n2 = getIkuroGamesSecuencial(game);
                if (n1 < 0)
                {
                    if ((n1 * -1) < n2)
                    {
                        precio = "oferta: $" + (n1 * -1) + "-$" + n2;
                    }
                    else if ((n1 * -1) == n2)
                    {
                        precio = "oferta: $" + n2; //ponemos solo uno porque son iguales
                    }
                    else
                    {
                        precio = "oferta: $" + n2 + "-$" + (n1 * -1);
                    }
                }
                else
                {
                    if (n1 < n2)
                    {
                        precio = "$" + n1 + "-$" + n2;
                    }
                    else if (n1 == n2)
                    {
                        precio = "$" + n2;  //ponemos solo uno porque son iguales
                    }
                    else
                    {
                        precio = "$" + n2 + "-$" + n1;
                    }
                }
                precio = game.Replace(" ", "").ToLower() + ";" + precio;
                listaPrecios.Add(precio);
            }
            return listaPrecios;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            listView1.Clear();
            this.listView1.View = View.LargeIcon;
            this.imageList2.ImageSize = new Size(105, 130);
            this.listView1.LargeImageList = this.imageList2;
            label2.Text = "";
            label6.Text = "";
            label8.Text = "";
            listaItems = new List<ListViewItem>();
            limpiarCarpeta(new DirectoryInfo(pathImages));
            //limpiar todo

            //reloj para cada proceso
            Stopwatch stopWatchPrecios = new Stopwatch();
            Stopwatch stopWatchPuntajes = new Stopwatch();
            Stopwatch stopWatchTiempos = new Stopwatch();

            List<string> listaPrecios=null;
            List<string> listaScores = null;
            List<Tuple<string, string>> listaTiempos = null;

            string t1="", t2="", t3="";

            Console.WriteLine("Inicio de Ejecucion Mutiproceso.");
            stopWatch.Start(); //proceso general o total

            Parallel.Invoke(() =>
            {
                getImages();
                Console.WriteLine("Imagenes descargadas correctamente.");
            }, () =>
            {
                stopWatchPrecios.Start();
                listaPrecios = getPreciosMutiproceso();
                t1 = stopWatchPrecios.Elapsed.TotalSeconds + "s";
                Console.WriteLine("Precios cargados correctamente.");
            }, () =>
            {
                stopWatchPuntajes.Start();
                listaScores = getMetacriticSecuencial();
                t2 = stopWatchPuntajes.Elapsed.TotalSeconds + "s";
                Console.WriteLine("Puntajes cargados correctamente.");
            }, () =>
            {
                stopWatchTiempos.Start();
                listaTiempos = getHowlogToBeatSecuencial();
                t3 = stopWatchTiempos.Elapsed.TotalSeconds + "s";
                Console.WriteLine("Tiempos cargados correctamente.");
            });

            label2.Text = t1;
            label6.Text = t2;
            label8.Text = t3;

            for (int i = 0; i < games.Count; i++)
            {
                if (listaPrecios[i].Split(';')[1].Length > 12)
                {
                    listaItems[i].BackColor = Color.Yellow;
                }
                listaItems[i].Text += "\n" + listaPrecios[i].Split(';')[1] + "\n" + " P:" + listaScores[i].Split(';')[1] + " - HLTB:" + listaTiempos[i].Item2 + "h";
                this.listView1.Items.Add(listaItems[i]);
            }
            Console.WriteLine("Datos ordenados correctamente.");
            Console.WriteLine("Tiempo Total: {0} segundos", stopWatch.Elapsed.TotalSeconds);
            Console.WriteLine("Fin de Ejecucion Mutiproceso.");
            
            stopWatch.Reset();
            stopWatchPrecios.Reset();
            stopWatchPuntajes.Reset();
            stopWatchTiempos.Reset();
        }
    }
}