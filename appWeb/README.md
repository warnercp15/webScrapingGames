# Estructura de la aplicación Web

**1. Servidor**

Se realizó un backend en Python utilizando el framework Flask. Dicho backend posee dos métodos principales, uno que espera un json que será enviado desde la aplicación de C# y el contenido es almacenado en una variale. El segundo método toma el contenido de la variable y lo retorna.

**2. Generador de datos**

Se creo una aplicación en C# para realizar la tarea de webscrapping a diferentes páginas web aplicando programación multicore, además esta aplicación se actualiza de una manerea constante, para así nutrir al servidor con los datos más recientes. Se escribió un script en Python para obtener la información de la página howlongtobeat.com implementando la API que existe para el lenguaje. La comunicación entre lenguajes (C# - Python) se realiza por medio de ficheros.

**3. Frontend**

El frontend fue desarrollado en Angular. Desde este módulo se hace consulta al servidor por el json generado en C#. Una vez obtenido el json array, se recorre para obtener la información que será mostrada en la vista web.

# Para ejecutar el proyecto tener en cuenta:

0. Instalar las dependencias necesarias con el comando 'npm i' (Angular), e instalar las librerías necesarias para correr el server 'pip install Flask-Cors' (Python). 
1. Cambiar la dirección IPv4 escrita en la función SendJsonAsync de la clase MultiprocessingMethods de C# por la dirección IPv4 correspondiente a tu computador.
2. Cambiar la dirección IPv4 para la función main del servidor escrito en Python por la dirección IPv4 correspondiente a tu computador.
3. Cambiar la dirección IPv4 en el archivo del Frontend app.component.ts en la función getData por la dirección IPv4 correspondiente a tu computador.

**Nota**: Esto de cambiar tu IPv4 es para poder correr el sistema de una manera local. Ejecutando en la consola de Windows el comando 'ipconfig' se podrá obtener la dirección IPv4 de tu computador.