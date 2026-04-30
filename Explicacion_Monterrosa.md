# Documentación de la Interfaz (Frontend) - BankTurns
**Rol: Monterrosa**

Este documento explica de forma detallada la arquitectura, diseño y el código de la capa de presentación (Vistas y Controladores MVC) desarrollada para el sistema de turnos. Toda esta implementación consume las APIs construidas por mis compañeros (servicios y base de datos) empleando `Fetch` de Javascript, lo cual permite una interfaz extremadamente rápida, dinámica y reactiva, sin necesidad de recargar la página.

## 1. Arquitectura de Controladores MVC

Se crearon tres controladores principales que funcionan como puertas de entrada (puntos de acceso) a las tres grandes pantallas del sistema:

1. **`KioskController.cs`**: Controlador que renderiza la vista del Panel de Turnos (`/Kiosk`).
2. **`WaitingRoomController.cs`**: Controlador encargado de renderizar la Sala de Espera pública (`/WaitingRoom`).
3. **`AdvisorPanelController.cs`**: Renderiza tanto la pantalla de login del asesor como el panel principal de atención (`/AdvisorPanel` y `/AdvisorPanel/Panel`).
4. **`HomeController.cs` (Modificado)**: Se actualizó para mostrar un menú principal en forma de tres grandes tarjetas interactivas que redirigen a los submódulos.

*¿Por qué Controladores MVC tradicionales si ya había Controladores API?*
El proyecto requiere separar la entrega de los datos (la API) de la interfaz de usuario. Mis controladores MVC sirven el HTML base, y desde allí el JavaScript en el cliente se encarga de llamar a los `[ApiController]` de mis compañeros para poblar la información, logrando un modelo híbrido muy eficiente.

## 2. Sistema de Diseño (`bank-theme.css`)

Se diseñó desde cero una hoja de estilos premium ubicada en `wwwroot/css/bank-theme.css`. Sus características son:
- **Estética Glassmorphism**: Las tarjetas tienen fondos translúcidos que simulan cristal, logrados mediante la propiedad `backdrop-filter: blur(20px)`.
- **Modo Oscuro (Dark Theme)**: Emplea una paleta de colores azul medianoche profunda (`#050d1a`), que brinda una sensación institucional seria, profesional y tecnológica.
- **Acentos Dorados**: Se utiliza el color oro (gold) para botones principales y números de ticket, logrando un contraste elegante (Premium).
- **Tipografía**: Incorpora fuentes nativas modernas como `Inter` (para textos legibles) y `Space Grotesk` (para titulares de alto impacto).
- **Notificaciones (Toast)**: Se programó una función global en el `_Layout.cshtml` llamada `showToast()` que hace aparecer pequeños mensajes flotantes (verde, azul o rojo) según el resultado de las acciones (ej: "Turno llamado exitosamente").

## 3. Lógica de las Vistas por Componente

### A. Kiosco (Panel de Turno)
El usuario ingresa su documento en un flujo dinámico de pasos ocultos:
- **Paso 1 (Identificación)**: Se consulta el documento llamando a `GET /api/Users/{document}`.
  - Si el usuario *ya existe*, se verifica si tiene turno activo. Si no tiene, se le crea un turno y se pasa directo al Paso 3 (Ticket).
  - Si el usuario *no existe* (Error 404 de la API), el JS detecta esto y muestra el Paso 2.
- **Paso 2 (Registro)**: Solicita Nombre y Motivo de visita. Llama a `POST /api/Users`, y al tener éxito genera el turno automáticamente.
- **Paso 3 (Generación e Impresión Física)**: Renderiza en pantalla el Ticket. Simultáneamente, hace un request HTTP POST asíncrono a la impresora local (provista por los mentores en la red WIFI RIWI) con los datos del turno formateados:
  ```javascript
  await fetch('http://10.0.11.2:5001/Home/Print', {
      method: 'POST',
      body: JSON.stringify({ content: "Turno: A-01\nCliente: Juan" })
  });
  ```

### B. Sala de Espera
Es un "Dashboard" público dividido en dos columnas:
- **Columna Izquierda (Video)**: Contiene un iFrame configurado para reproducir automáticamente de manera silenciada un playlist o video promocional del banco.
- **Columna Derecha (Cola en Tiempo Real)**:
  - **Polling Dinámico**: El Javascript ejecuta `setInterval()` para solicitar la lista de turnos mediante `GET /api/Turns/queue` cada 3 segundos. Así, la pantalla se actualiza "mágicamente" sin intervención humana cuando el Asesor llama a alguien.
  - **Texto a Voz (TTS)**: El navegador detecta cuando un ticket nuevo es pasado al estado de *En Atención* y utiliza la API nativa `speechSynthesis` del navegador para anunciar por altavoces: *"Turno A-01, por favor, pase a la ventanilla"*.

### C. Panel de Asesor
Es el sistema administrativo, con seguridad basada en `sessionStorage`:
- **Login**: Inicia sesión consultando `POST /api/Advisors/login`. Guarda el ID en la memoria temporal del navegador.
- **Panel Interactivo**:
  - Muestra tarjetas de estadísticas superiores (En Cola, En Atención, Finalizados).
  - Listado de espera actualizado en tiempo real.
  - Botón **"Llamar Siguiente"**: Invoca a `POST /api/Turns/call-next` e inmediatamente actualiza la tabla.
  - Botón **"Finalizar Turno"**: Despliega una ventana modal (creada con CSS y JS puro) para agregar un comentario sobre la gestión del cliente antes de cerrar su turno definitivamente mediante `POST /api/Turns/{id}/finish`.

---
**Conclusión**  
La integración realizada cumple a cabalidad los requisitos del sistema: es altamente responsiva, se conecta exitosamente con la arquitectura del backend construida por el equipo, atiende flujos de impresión física y TTS, y eleva el nivel de experiencia del usuario final gracias al diseño premium y asíncrono.
