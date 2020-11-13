using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System.Net;
using System.Diagnostics;
using System.Globalization;
using Newtonsoft.Json;
using System.Net.Http;

namespace webScrapingGames
{
    class MultiprocessingMethods
    {
        List<string> games = new List<string>() { "uncharted the nathan drake collection", "final fantasy vii remake", "sekiro shadows die twice", "devil may cry 5", "the evil within 2", "borderlands 3", "cuphead", "doom eternal", "fallout 76", "fifa 21", "payday 2 crimewave edition", "middle earth shadow of mordor", "ark survival evolved", "red dead redemption 2", "ghost of tsushima", "tomb raider definitive edition", "need for speed payback", "resident evil 0 hd", "lego marvel super heroes", "batman arkham knight", "lego marvel avengers", "lego harry potter collection", "street fighter v", "until dawn", "bloodborne", "heavy rain", "dying light", "dishonored 2", "just cause 4", "ufc 3", "doom", "days gone", "resident evil 5", "resident evil hd", "mad max", "dirt 4", "tekken 7", "lego worlds", "mortal kombat x", "battlefield 4" };
        Stopwatch stopWatch = new Stopwatch();

        private List<string> getMetacriticSecuencial()
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

        private List<string> getImages()
        {
            List<string> imagesUrls = new List<string>();
            HtmlWeb oWeb = new HtmlWeb();
            WebClient oClient = new WebClient();
            HtmlAgilityPack.HtmlDocument doc;
            string url;
            foreach (var nombre in games)
            {
                url = "http://dixgamer.com/shop/juegos/ps4/accion-ps4/" + nombre.Replace(" ", "-") + "/?v=1d7b33fc26ca";
                doc = oWeb.Load(url);
                foreach (var Nodo in doc.DocumentNode.CssSelect("img.wp-post-image"))
                {
                    imagesUrls.Add(Nodo.GetAttributeValue("data-src"));
                    break;
                }
            }
            return imagesUrls;
        }

        private float getDixGamerGamesSecuencial(string nombre)
        {
            float price = 0;
            Boolean oferta = false;
            string resultado, auxPrice;
            HtmlWeb oWeb = new HtmlWeb();
            HtmlAgilityPack.HtmlDocument doc;
            //Console.WriteLine("Buscando: " + nombre + " en DixGamer");
            string url = "http://dixgamer.com/shop/juegos/ps4/accion-ps4/" + nombre.Replace(" ", "-") + "/?v=1d7b33fc26ca";
            doc = oWeb.Load(url);
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
                if (resultado.Length > 5)
                {
                    auxPrice = "" + resultado[4] + resultado[5] + resultado[6] + resultado[7];
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
                else if (precios.Length == 8)
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
            price = price / float.Parse("79,0307"); //se convierte a dolares porque viene en ARS (pesos argentinos)
            aux = price.ToString("####0.00");
            return float.Parse(aux);
        }

        private List<string> getPreciosMutiproceso()
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

            for (int i = 0; i < games.Count; i++)
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

        private List<Tuple<string, string>> GetHowlogToBeatSecuencial()
        {
            HowLongToBeatPy howLongToBeat = new HowLongToBeatPy(games);      // Instancia de la clase que realiza las consultas
            // Función principal de hltb retorna una lista de tuplas L = [(nombrejuego_1, tiempo_1),...,(nombrejuego_n, tiempo_n)]
            List<Tuple<string, string>> hltbResponse = howLongToBeat.GetTimeToBeat();
            return hltbResponse;
        }

        public async Task ExecuteMultiproccessingAsync()
        {
            List<string> listaPrecios = null;
            List<string> listaScores = null;
            List<Tuple<string, string>> listaTiempos = null;
            List<string> listaImgs = null;

            string price, name, score, timeToBeat, imgUrl;
            Boolean offer;
            Console.WriteLine("\nInicio de Ejecucion Mutiproceso.");
            stopWatch.Start(); //proceso general o total

            Parallel.Invoke(() =>
            {
                listaImgs = getImages();
                Console.WriteLine("Imagenes descargadas correctamente.");
            }, () =>
            {
                listaPrecios = getPreciosMutiproceso();
                Console.WriteLine("Precios cargados correctamente.");
            }, () =>
            {
                listaScores = getMetacriticSecuencial();
                Console.WriteLine("Puntajes cargados correctamente.");
            }, () =>
            {
                listaTiempos = GetHowlogToBeatSecuencial();
                Console.WriteLine("Tiempos cargados correctamente.");
            });
            Console.WriteLine("Datos recopilados con éxito");

            List<GameInfo> gamesInformation = new List<GameInfo>();

            for (int i = 0; i < games.Count; i++)
            {
                if (listaPrecios[i].Split(';')[1].Length > 12)
                {
                    offer = true;
                    price = listaPrecios[i].Split(';')[1].Split(':')[1];
                }
                else
                {
                    offer = false;
                    price = listaPrecios[i].Split(';')[1];
                }
                name = games[i];
                score = listaScores[i].Split(';')[1];
                timeToBeat = listaTiempos[i].Item2 + 'h';
                imgUrl = listaImgs[i];
                GameInfo gameInfo = new GameInfo(name, offer, price, score, timeToBeat, imgUrl);
                gamesInformation.Add(gameInfo);
            }

            var json = JsonConvert.SerializeObject(new
            {
                data = gamesInformation
            });

            await SendJsonAsync(json);

            Console.WriteLine("Datos ordenados correctamente.");
            Console.WriteLine("Tiempo Total: {0} segundos", stopWatch.Elapsed.TotalSeconds);
            Console.WriteLine("Fin de Ejecucion Mutiproceso.");

            stopWatch.Reset();
        }

        public async Task SendJsonAsync(string json)
        {
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    "tuIpv4/json",
                     new StringContent(json, Encoding.UTF8, "application/json"));
            }
        }
    }
}
