# AI-Driven Procedural Rogue-lite (Dynamic Difficulty Adjustment)

Este repositorio contiene el código fuente y la build jugable de mi Trabajo de Fin de Grado en Ingeniería Informática. Se trata de un videojuego de acción 2D que ajusta su dificultad en tiempo real (DDA) estudiando la telemetría del jugador mediante Machine Learning (Support Vector Machines) y alterando el entorno con Procedural Content Generation (PCG).

## Arquitectura del Sistema
El proyecto utiliza una arquitectura cliente-servidor desacoplada para garantizar el rendimiento gráfico:
* **`TFG - Legions of Rome/`**: Cliente desarrollado en Unity (C#). Es el videojuego, recopila métricas de combate (APM, precisión, tiempo en zonas de riesgo) y ejecuta la generación procedimental.
* **`TFG - Machine Learning/`**: API REST desarrollada con FastAPI y `scikit-learn`. Procesa el JSON de telemetría, clasifica el arquetipo del jugador y devuelve la predicción en tiempo real.

## Requisitos e Instalación
El sistema consta de dos partes que deben ejecutarse simultáneamente. 

### 1. Levantar el Servidor de IA (Backend)
Requiere Python 3.8 o superior.
```bash
cd '.\TFG - Joc Final\Backend_IA\'
pip install -r requirements.txt
python api.py

```

*(Deja esta ventana de la terminal abierta en segundo plano).*

### 2. Ejecutar el Juego (Frontend)

Ve a la carpeta `TFG - Joc Final/Build_TFG` y abre `TFG - Legions of Rome.exe`.
En el menú principal verás distintos modos. Una "sesión" o "run" acaba cuando mueres o cuando superas la Sala 15.

## Controles

El juego es totalmente compatible tanto con Teclado/Ratón como con Mando (Xbox / PlayStation). **Se recomienda jugar con mando**.

| Acción | Teclado y Ratón | Mando (Xbox / PlayStation) |
| --- | --- | --- |
| **Moverse** | `WASD` | Joystick Izquierdo |
| **Ataque Cuerpo a Cuerpo** | Clic Izquierdo | Botón Oeste (`X` / `Cuadrado`) |
| **Ataque a Distancia** | `F` | Botón Norte (`Y` / `Triángulo`) |
| **Esquivar (Dash)** | `Espacio` | Botón Superior Derecho (`RB` / `R1`) |
| **Activar Escudo** | Clic Derecho | Botón Superior Izquierdo (`LB` / `L1`) |
| **Pausa** | `Esc` | `Start` |

## Extracción de Datos

Si deseas analizar tu rendimiento, el juego guarda automáticamente tu telemetría en un archivo CSV local en la siguiente ruta:
`%userprofile%\AppData\LocalLow\UAB_Eduard\TFG - Legions of Rome\validation_results.csv`

## Autor

Desarrollado por **Eduard José García Mendeleac**.
