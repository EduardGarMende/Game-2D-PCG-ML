¡Hola! Gracias por participar en las pruebas A/B de mi TFG.

El sistema consta de dos partes que deben ejecutarse simultáneamente: el motor de Inteligencia Artificial (Backend) y el Juego (Frontend).

PASO 1: Levantar el Servidor de IA

Abre una terminal en la carpeta Backend_IA.

Instala las dependencias ejecutando: pip install -r requirements.txt

Arranca el servidor ejecutando: python api.py
(Deja esta ventana de terminal abierta en segundo plano).

PASO 2: Jugar

Ve a la carpeta Build_TFG y abre TFG - Legions of Rome.exe.

En el menú principal verás tres modos. Necesito que juegues partidas en modo normal y sin DDA para comparar tu rendimiento.

Una "sesión" o "run" acaba cuando mueres o cuando superas la Sala 15.

CONTROLES DEL JUEGO
El juego es totalmente compatible tanto con Teclado/Ratón como con Mando (Xbox / PlayStation). Se recomienda jugar con mando para una mejor experiencia.

┌──────────────────────────┬──────────────────────┬───────────────────────────────────┐
│ Acción                          │ Teclado y Ratón            │ Mando (Xbox / PlayStation)                  │
├──────────────────────────┼──────────────────────┼───────────────────────────────────┤
│ Moverse                         │ WASD                       │ Joystick Izquierdo                          │
│ Ataque Cuerpo a Cuerpo          │ Clic Izquierdo             │ Botón Oeste (X / Cuadrado)                  │
│ Ataque a Distancia              │ F                          │ Botón Norte (Y / Triangulo)                 │
│ Esquivar (Dash)                 │ Espacio                    │ Botón Superior Derecho (RB / R1)            │
│ Activar Escudo                  │ Clic Derecho               │ Botón Superior Izquierdo (LB / L1)          │
│ Pausa                           │ Esc                        │ Start                                       │
└──────────────────────────┴──────────────────────┴───────────────────────────────────┘

PASO 3: Extraer y Enviarme los Datos
El juego guarda automáticamente toda tu telemetría en un archivo CSV local. Al terminar todas tus partidas, ve a esta ruta exacta en tu explorador de archivos de Windows:

%userprofile%\AppData\LocalLow\UAB_Eduard\TFG - Legions of Rome\validation_results.csv

Cópialo y envíamelo. ¡Mil gracias por la ayuda!