# Metabuscador de videojuegos con Web scrapping

Este proyecto consiste en el desarrollo de un metabuscador de videojuegos utilizando Web scrapping y programacion multicore con el objetivo de aprender el potencial que brindan estas herramientas en un sistema de computo.

Enlace de la aplicacion Web: https://webscrapinggames.web.app/

Esta aplicacion web esta en la carpeta nombrada como "appWeb" en este repositorio.

**Resultado**

![resultado](<./assets/resultado.jpeg>) 

# Web scrapping

La técnica del WebScrapping consiste en extraer información de diferentes sitios web, esta tecnica fue determinante en el desarrollo de este proyecto y su implementación esta basada principalmente en librerías que dispone C# para realizar solicitudes a una pagina, en caso, utilizamos HtmlAgilityPack y HowLongToBeatPy API.

Estas librerías permiten definir mediante las etiquetas de los sitios web que información se desea extraer, por lo tanto, con la información ya obtenida, esta se puede analizar y mostrar segun los requerimientos del software. Cabe destacar que, esta técnica NO puede ser aplicada a cualquier sitio web, algunos poseen bloqueos ante esta.

# Programacion Multicore

Para implementar la programacion multicore se emplearon 4 niveles de paralelismo:

**1º Nivel de paralelismo**

Se creó una lista de tipo String en C# que contiene los nombres de todos los juegos, por cada juego se inicializan 4 procesos de Web scrapping en paralelo para obtener sus diferentes datos (Imagen, precio, puntaje, duración), por lo que esta lista de juegos es recorrida por cada proceso de forma paralela. Cabe destacar, que los datos obtenidos en cada proceso para cada juego se va guardando en una lista del respectivo proceso.

**2º Nivel de paralelismo**

De manera paralela se consultan los siguientes sitios web para obtener los datos de cada juego (Imagen, precio, puntaje, duración):

Ikurogames, Dixgamer, Metacritic: Para implementar el Web scrapping en estos sitios web se implementaron peticiones con HtmlAgilityPack mediante C# 

HowLongToBeat: Para implementar el Web scrapping se utilizó un API de Python (HowLongToBeat Python API), esta es ejecutada desde C#. Cabe destacar que esta API es necesaria porque el sitio web posee un bloqueo ante esta técnica.

**3º Nivel de paralelismo**

De manera paralela se consulta el precio de cada juego en Ikurogames y Dixgamer, para así compararlos y clasificar cual sitio web posee el precio más bajo y cual el precio más alto, a la vez, se detecta si alguno de los 2 sitios posee alguna oferta en este juego, para luego catalogarlo de esta manera. 

**4º Nivel de paralelismo**

En cada página se consultan 40 juegos, por lo que se implementó otro proceso en paralelo para dividir estas consultas en 2 procesos, de forma que cada uno posee 20 juegos, esto se implementó para obtener una mejoría en los tiempos de respuesta.

# Diferencias de la programacion multicore y la programacion secuencial

Con el objetivo de determinar la diferencia entre utilizar la típica programación secuencial a utilizar la programación multicore se implementó la posibilidad de ejecutar esta aplicación de forma secuencial, de forma que se ejecuta cada instrucción hasta que se finalice la anterior, esto mostró un aumento en los tiempos de respuesta bastantes significativos con respecto a la programación multicore.

Para esto se desarrollo una aplicacion secundaria de esritorio que permite ejecutar el programa de forma secuencial o multicore como el usuario lo prefiera, esto mas que todo con fines educativos, para demostrar la diferencia de rendimiento que poseen estos tipos de programacion y asi obtener mayor provecho de este proyecto.

Esta aplicacion de escritorio esta en la carpeta nombrada como "appEscritorio" del presente repositorio.

**Resultado**

![vistaEscritorio](<./assets/vistaEscritorio.jpeg>)

**Tiempo de ejecucion secuencial**

![secuencial](<./assets/secuencial.jpeg>)

**Tiempo de ejecucion multicore**

![multicore](<./assets/multicore.jpeg>)

# Conclusiones

1. La técnica del WebScrapping resulta ser muy útil para extraer una gran cantidad de información de diferentes fuentes en poco tiempo y así visualizarlas en un solo lugar lo que permite realizar una comparación de diversos sitios para elegir la mejor opción dependiendo de las variables contempladas.

2. Contemplando los resultados obtenidos en la aplicacion de escritorio y la aplicacion web se puede destacar como la programación multicore provee una mejora significativa en los tiempos de respuesta de un programa, lo cual en el mercado actual resulta indispensable para competir, ya que el rendimiento es fundamental.
