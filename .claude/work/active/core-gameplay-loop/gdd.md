# GDD: Core Gameplay Loop

## 1. Overview

Space Roguelike es un shooter top-down de naves donde cada run es un viaje fresco a través de 10 sectores espaciales de dificultad creciente. El jugador elige un arquetipo de nave al inicio, acumula armas entre niveles, y combate oleadas de enemigos hasta completar el nivel 10 o morir en el intento. La muerte es permanente — cada run empieza desde cero. Las sesiones duran 10–15 minutos, diseñadas para partidas en pausas o traslados. La sensación buscada es la de *Soul Knight*: movimiento activo en un mapa amplio, variedad de enemigos con comportamientos distintos, y la satisfacción de construir un build poderoso nivel a nivel. El mapa de cada nivel es de **gran tamaño fijo** — la cámara sigue a la nave y hace scroll al acercarse a los bordes de la pantalla, dando sensación de espacio abierto. Los enemigos siempre llegan desde fuera del mapa, lo que obliga al jugador a gestionar su posición constantemente.

---

## 2. Player Experience Goal

- **Sentirse una amenaza creciente** — fragil al inicio, devastador con el build correcto en nivel 10
- **Sentir tensión real** cuando la vida es baja y todavía quedan enemigos
- **Sentir satisfacción táctica** al elegir armas que se complementan
- **Entender siempre por qué murió** — la curva gradual garantiza que el nivel 1 enseña y el nivel 10 desafía

---

## 3. Core Loop

1. El jugador selecciona un arquetipo de nave (tanque / daño / velocidad) en la pantalla de inicio
2. Comienza el nivel 1 — los enemigos llegan desde los bordes exteriores del mapa en oleadas o de forma continua; el jugador los ve entrar desde fuera del área visible
3. El jugador mueve la nave con el joystick izquierdo y apunta con el joystick derecho; la nave dispara automáticamente en la dirección apuntada si hay un enemigo dentro del rango de ataque
4. El jugador elimina a todos los enemigos → el nivel termina
5. Aparece la pantalla de selección de arma: se presentan 2 cartas de arma al azar; el jugador elige 1
   - Si ya tiene 3 armas activas, debe intercambiar o mejorar una existente
6. Comienza el siguiente nivel con enemigos más fuertes y numerosos
7. Al completar el nivel 10 → pantalla de victoria → regreso al menú
8. Si la vida llega a 0 en cualquier momento → pantalla de muerte → el jugador puede reiniciar (nueva run) o volver al menú

---

## 4. Player Actions

| Acción | Input | Resultado |
|--------|-------|-----------|
| Mover la nave | Joystick virtual (pulgar izquierdo) | La nave se desplaza en cualquier dirección |
| Apuntar | Joystick virtual (pulgar derecho) | Define la dirección de disparo |
| Disparar | Automático al apuntar | Si hay un enemigo dentro del rango en la dirección apuntada, la nave dispara; si no hay enemigo en rango, no dispara |
| Seleccionar arma | Tap en carta (entre niveles) | El arma se agrega al slot activo |
| Intercambiar arma | Tap en carta + tap en slot a reemplazar | El arma vieja se descarta, la nueva se equipa |

---

## 5. Game Responses & Feedback

### Combate

| Evento | Feedback visual | Feedback de audio |
|--------|----------------|-------------------|
| Disparo propio | Proyectil sale de la nave con trail | SFX de disparo (varía por arma) |
| Impacto en enemigo | Flash blanco en el enemigo, número de daño flotante | SFX de impacto |
| Enemigo muerto | Explosión pequeña, el sprite desaparece | SFX de explosión |
| El jugador recibe daño | Flash rojo en la nave, barra de vida baja | SFX de impacto en nave |
| Vida baja (<25%) | Barra de vida pulsa en rojo | SFX de alarma suave continuo |
| Enemigo entra al mapa | Sprite aparece desde el borde exterior con trail de entrada | SFX de aparición (warp/thrust) |
| Cámara hace scroll | El fondo del mapa se desplaza suavemente siguiendo la nave | — |
| Nivel completado | Todos los enemigos explotan, texto "NIVEL X COMPLETADO" | Fanfarria corta |
| Jugador muere | La nave explota, fundido a negro | SFX de explosión grande |
| Paquete de vida aparece | Ícono de corazón flotante cae del enemigo derrotado | SFX de item drop suave |
| Paquete de vida recogido | Flash verde en la nave, barra de vida sube | SFX de curación |
| Robo de vida (arma vampírica) | Número verde flotante sobre la nave al impactar | SFX sutil de absorción |

### Armas activas

- Máximo 3 armas activas simultáneas
- Cada arma dispara de forma independiente según su propia cadencia
- Todas las armas disparan en la dirección que el jugador apunta, si hay un enemigo en rango
- La nave inicia con un arma básica en el slot 1 que **puede intercambiarse** — el jugador puede quedar desarmado si elige reemplazarla sin tener otra
- Algunas armas tienen el atributo **vampírico**: cada impacto restaura una pequeña cantidad de vida

### Paquetes de vida

- Al morir un enemigo, existe una probabilidad del **0.3%** de que suelte un paquete de vida
- El paquete queda en el mundo como un objeto coleccionable; el jugador debe pasar sobre él para recogerlo
- Si no se recoge antes de que termine el nivel, desaparece
- El mapa es de **tamaño fijo y grande** — la cámara hace scroll siguiendo al jugador, pero el mapa tiene límites físicos que la nave no puede cruzar

### Tipos de enemigos

Cada tipo tiene una identidad de combate clara. El jugador aprende a leerlos visualmente por su forma o color.

| Tipo | Rango | Cadencia | Daño | Vida | Velocidad | Comportamiento especial |
|------|-------|----------|------|------|-----------|------------------------|
| **Estándar** | Normal | Normal | Normal | Normal | Normal | Avanza hacia el jugador, dispara |
| **Ametrallador** | Corto | Alta | Bajo | Normal | Normal | Se acerca agresivamente para entrar en rango |
| **Francotirador** | Largo | Baja | Alto | Normal | Normal | Mantiene distancia máxima, dispara proyectil lento pero poderoso |
| **Chaser (Kamikaze)** | — | — | Alto | Normal | Alta | No dispara; persigue al jugador en línea recta y explota al contacto |
| **Warper** | Normal | Normal | Normal | Normal | Normal | Se teletransporta a posiciones aleatorias del mapa cada pocos segundos |
| **Tanque** | Normal | Baja | Alto | Muy alta | Baja | Absorbe mucho daño; lento pero su proyectil hace daño significativo |

> Los tipos que aparecen en cada nivel se determinan aleatoriamente por run, con restricciones: Chasers y Warpers no aparecen en los primeros niveles hasta que el jugador haya tenido tiempo de aprender el sistema de movimiento.

### Inteligencia artificial por nivel

A medida que suben los niveles, los enemigos no solo escalan en stats — también se vuelven **más inteligentes y coordinados**. Esto aplica a todos los tipos excepto Chaser (que siempre carga en línea recta) y Warper (que siempre teletransporta).

| Niveles | Comportamiento de IA |
|---------|---------------------|
| 1–3 | Movimiento directo hacia el jugador; sin coordinación entre enemigos; no esquivan proyectiles |
| 4–5 | Esquivan proyectiles ocasionalmente (reacción con delay); mantienen la distancia óptima de su tipo de disparo |
| 6–7 | Se organizan en grupos de 2–3; intentan flanquear acercándose por distintos ángulos simultáneamente |
| 8–9 | Rodean activamente al jugador; los grupos coordinan el momento del rush; Chasers se sincronizan para llegar al mismo tiempo |
| 10 | Comportamiento máximo: combinan flanqueo, rodeo, coordinación de rush y esquiva activa; el jugador nunca tiene un ángulo libre |

### Escalado de enemigos por nivel

| Nivel | HP | Velocidad | Cadencia de disparo | Cantidad de enemigos |
|-------|----|-----------|---------------------|----------------------|
| 1 | 1.0× | 1.0× | 1.0× | ~5 |
| 2–3 | 1.2× | 1.1× | 1.1× | ~7–9 |
| 4–5 | 1.5× | 1.25× | 1.3× | ~11–13 |
| 6–7 | 1.8× | 1.4× | 1.5× | ~15–17 |
| 8–9 | 2.2× | 1.6× | 1.8× | ~19–21 |
| 10 | 2.5× | 1.75× | 2.0× | ~23 |

---

## 6. Win / Lose / Progression Conditions

- **Ganar nivel:** Matar a todos los enemigos del nivel
- **Ganar run:** Completar los 10 niveles sin morir
- **Perder:** La vida de la nave llega a 0 — permadeath, la run termina completamente
- **Progresión dentro de la run:** El jugador crece a través de la selección de armas entre niveles
- **Sin progresión entre runs:** No hay desbloqueos permanentes — cada run parte desde cero con el arquetipo elegido

---

## 7. UI & Screen Flow

```
MainMenu
  └─ [Jugar] → Ship Selection (3 arquetipos)
                  └─ [Confirmar] → Level 1
                                    └─ [Todos muertos] → Weapon Selection
                                                           └─ [Elegir arma] → Level 2
                                                                               └─ ...
                                                                                    └─ Level 10
                                                                                         └─ [Todos muertos] → Victory Screen
                                                                                                               └─ [Menú] → MainMenu
                                    └─ [HP = 0] → Death Screen
                                                    ├─ [Reintentar] → Ship Selection
                                                    └─ [Menú] → MainMenu
```

**Pantallas involucradas:**
- **MainMenu** — título, botón de jugar, opciones básicas
- **Ship Selection** — 3 cartas de arquetipo con stats básicos (HP, daño, velocidad)
- **Gameplay HUD** — barra de vida, contador de enemigos restantes ("3 enemies left"), número de nivel, slots de armas activas
- **Weapon Selection** — 2 cartas de arma con nombre, ícono y **stats numéricos** (daño, cadencia, rango, efectos especiales); si hay 3 armas activas, muestra los slots actuales para que el jugador elija cuál reemplazar
- **Death Screen** — "Has muerto en el nivel X", opciones de reintentar o menú
- **Victory Screen** — "Run completada", estadísticas básicas (niveles completados, enemigos eliminados)

---

## 8. Edge Cases

| Situación | Comportamiento diseñado |
|-----------|------------------------|
| El jugador llega al nivel con 3 armas y selecciona una 4ª | La pantalla de selección muestra los 3 slots actuales; el jugador debe tocar uno para reemplazarlo |
| Dos enemigos a la misma distancia | La nave elige uno de forma consistente (p. ej. el primero en la lista interna) — no aleatorizar cada frame para evitar temblor de disparo |
| Arma de área elimina varios enemigos a la vez | El nivel termina normalmente cuando el contador llega a 0 |
| El jugador se mueve hacia el borde del mapa | El mapa tiene límites físicos visibles (nebulosa, campo de asteroides); la cámara deja de hacer scroll al llegar al borde; la nave no puede salir |
| Un enemigo entra al mapa mientras la cámara no apunta a ese borde | El enemigo aparece fuera de pantalla; el jugador puede inferir su posición por el minimap o indicadores de borde (flechas en los márgenes de pantalla) |
| Chaser explota cerca de otros enemigos | La explosión solo daña al jugador — no hay daño en cadena entre enemigos |
| Warper se teletransporta encima del jugador | Se define una zona de exclusión: el Warper no puede aparecer a menos de X unidades del jugador |
| Chaser y Francotirador en el mismo nivel | El Chaser presiona al jugador a moverse, impidiendo que el Francotirador sea esquivado fácilmente — combinación diseñada para niveles altos |
| El jugador intercambia su única arma y queda desarmado | La nave no dispara — el jugador debe sobrevivir moviéndose; es una decisión táctica de riesgo válida |
| El jugador no apunta (joystick derecho en reposo) | La nave no dispara — requiere input activo de apuntado |
| Paquete de vida cae pero el nivel termina antes de recogerlo | El paquete desaparece al finalizar el nivel; no se traslada al siguiente |

---

## 9. Decisions Resolved

| # | Pregunta | Decisión |
|---|----------|----------|
| 1 | ¿El disparo tiene rango máximo? | **Sí.** El jugador apunta con joystick derecho; si no hay enemigo en rango en esa dirección, la nave no dispara. Es un shooter twin-stick, no auto-aim puro. |
| 2 | ¿El jugador recupera vida? | **No hay curación entre niveles.** La curación ocurre en combate: armas vampíricas (robo de vida al impactar) y paquetes de vida con 0.3% de drop al morir un enemigo. |
| 3 | ¿El arena escala? | **Tamaño fijo y grande.** La cámara hace scroll siguiendo al jugador. Los enemigos siempre llegan desde fuera del mapa. El mapa no cambia de tamaño entre niveles. |
| 4 | ¿Stats en selección de arma? | **Stats numéricos** — daño, cadencia, rango y efectos especiales visibles en la carta. |
| 5 | ¿El arma default es permanente? | **No.** El jugador inicia con un arma en el slot 1 que puede intercambiar libremente. Puede quedar desarmado si decide reemplazarla. |

---

## 10. Out of Scope

- Catálogo específico de armas (tipos, stats, efectos visuales) — requiere GDD propio
- Stats exactos de cada arquetipo de nave — requiere GDD propio
- Meta-progresión entre runs (monedas, desbloqueos permanentes, skins)
- Encuentros con jefes (boss levels)
- Diseño de audio detallado (SFX específicos, música por nivel)
- Efectos de partículas y juice detallado
- Multijugador
