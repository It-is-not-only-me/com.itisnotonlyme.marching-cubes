# Documentacion
---

Esta dividido en 3 clases/struct
 * MarchingCubesRenderer
 * GeneradorDatos
 * Dato

### MarchingCubesRenderer y GeneradorDatos
---
Ambos los podemos pensar como el equivalente a un mesh renderer y la mesh en si. La clase GeneradorDatos seria la mesh en esta analogia, es el encargado de dar todo los datos para el algoritmo de marching cube.
Estos datos serian un array tridimencional comprimido a un vector unidimencional y cuantos puntos por eje.

### Datos
---
Este struct es lo que se va a mandar por GeneradorDatos, que seria una posicion, un valor (valor que usa el algoritmo para determinar si esta debajo de la superficie o no), y por ultimo una coordenada uv, usada para lo que se quiera.