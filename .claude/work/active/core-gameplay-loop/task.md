# Core Gameplay Loop

## Juego

**Space Roguelike** — shooter top-down de naves

**Tipo:** Rogue-like (permadeath, generación aleatoria)

## Descripción

- El jugador controla una nave desde vista top-down
- Cada nivel: matar X enemigos para avanzar
- 10 niveles por run
- Al completar un nivel: seleccionar una de dos armas (máximo 3 activas), luego intercambiar o mejorar
- Al morir: empieza desde cero
- Enemigos escalan por nivel: más vida, más velocidad, disparan más rápido
- Generación de enemigos aleatoria en cada run
- 3 tipos de nave seleccionable al inicio (tanque, daño, velocidad) — escalable

## Acceptance Criteria

- [ ] El jugador puede moverse en todas direcciones
- [ ] El jugador dispara automáticamente al enemigo más cercano
- [ ] Los enemigos aparecen en cantidad y posición aleatoria
- [ ] Al matar todos los enemigos el nivel termina
- [ ] Al completar el nivel aparece la selección de arma
- [ ] Al morir regresa a la pantalla de inicio
- [ ] Los stats de enemigos escalan correctamente por nivel
