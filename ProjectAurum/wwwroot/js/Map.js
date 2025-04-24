
            // Función para desplazarse al mapa
        function scrollToMapa() {
                const mapaSection = document.getElementById("map");
        mapaSection.scrollIntoView({behavior: "smooth" });
            }

        let mapa, marcadorUsuario, marcadorLocal;

        // Coordenadas del local
        const localLat = 9.99595475445175;
        const localLng = -84.11213580974817;

        // Inicializar el mapa
        function inicializarMapa() {
            // Crear el mapa centrado en el local
            mapa = L.map('map').setView([localLat, localLng], 15);

        // Agregar capa de OpenStreetMap
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; OpenStreetMap contributors'
                }).addTo(mapa);

        // Agregar marcador para el local
        marcadorLocal = L.marker([localLat, localLng]).addTo(mapa)
        .bindPopup("Ubicación del Local")
        .openPopup();
            }

        // Mostrar la ubicación actual del usuario en el mapa
        function mostrarMiUbicacion() {
                if (!navigator.geolocation) {
            alert("Geolocalización no soportada.");
        return;
                }

        navigator.geolocation.getCurrentPosition(
                    (pos) => {
                        const lat = pos.coords.latitude;
        const lng = pos.coords.longitude;

        // Agregar o actualizar el marcador del usuario
        if (marcadorUsuario) {
            marcadorUsuario.setLatLng([lat, lng]);
                        } else {
            marcadorUsuario = L.marker([lat, lng]).addTo(mapa)
                .bindPopup("Tu ubicación")
                .openPopup();
                        }

        // Centrar el mapa en la ubicación del usuario
        mapa.setView([lat, lng], 15);
                    },
                    (err) => {
            alert("Error al obtener ubicación: " + err.message);
                    }
        );
            }

        // Mostrar la ubicación del local en el mapa
        function mostrarUbicacionLocal() {
            // Centrar el mapa en la ubicación del local
            mapa.setView([localLat, localLng], 15);

        // Asegurarse de que el marcador del local esté visible
        if (!marcadorLocal) {
            marcadorLocal = L.marker([localLat, localLng]).addTo(mapa)
                .bindPopup("Ubicación del Local")
                .openPopup();
                } else {
            marcadorLocal.openPopup();
                }
            }

        // Redirigir a Waze con la ubicación del local
        function redirigirAWaze() {
                const url = `https://waze.com/ul?ll=${localLat},${localLng}&navigate=yes`;
        window.open(url, "_blank");
            }

            // Inicializar el mapa al cargar la página
            window.onload = () => {
            inicializarMapa();
            };
 