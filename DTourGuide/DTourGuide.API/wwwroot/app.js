const MAP_CENTER = [18.473, -69.884];
const map = L.map('map').setView(MAP_CENTER, 15);

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; OpenStreetMap'
}).addTo(map);

let markers = [];
const categorySel = document.getElementById('category');
const searchInput = document.getElementById('search');
const btnFilter = document.getElementById('btnFilter');
const btnFavs = document.getElementById('btnFavs');
const favList = document.getElementById('favList');
const FAV_KEY = 'dtour_favs';

function getFavs() { return JSON.parse(localStorage.getItem(FAV_KEY) || '[]'); }
function setFavs(list) { localStorage.setItem(FAV_KEY, JSON.stringify(list)); renderFavs(); }

function addMarker(p) {
    const m = L.marker([p.latitude, p.longitude]).addTo(map);
    const favs = getFavs();
    const isFav = favs.includes(p.id);
    const btn = `<button data-id="${p.id}" class="fav-btn">${isFav ? '★ Quitar' : '☆ Favorito'}</button>`;
    m.bindPopup(`
    <b>${p.name}</b><br/>
    <i>${p.category}</i><br/>
    <small>${p.latitude.toFixed(5)}, ${p.longitude.toFixed(5)}</small><br/>
    ${btn}
  `);
    m.on('popupopen', () => {
        document.querySelector('.fav-btn')?.addEventListener('click', (e) => {
            const id = Number(e.target.getAttribute('data-id'));
            toggleFav(id);
            m.closePopup();
        });
    });
    markers.push(m);
}

function clearMarkers() {
    markers.forEach(m => m.remove());
    markers = [];
}

async function loadPlaces() {
    const params = new URLSearchParams();
    if (categorySel.value) params.append('category', categorySel.value);
    if (searchInput.value) params.append('search', searchInput.value);
    const res = await fetch(`/api/places?${params.toString()}`);
    const data = await res.json();
    clearMarkers();
    data.forEach(addMarker);
}

function renderFavs() {
    const favs = getFavs();
    favList.innerHTML = '';
    favs.forEach(id => {
        const li = document.createElement('li');
        li.innerHTML = `<span>#${id}</span><button data-id="${id}" class="del">Quitar</button>`;
        favList.appendChild(li);
    });
    favList.querySelectorAll('.del').forEach(btn => {
        btn.addEventListener('click', () => toggleFav(Number(btn.getAttribute('data-id'))));
    });
}

function toggleFav(id) {
    const favs = getFavs();
    const i = favs.indexOf(id);
    if (i >= 0) favs.splice(i, 1); else favs.push(id);
    setFavs(favs);
    loadPlaces();
}

btnFilter.addEventListener('click', loadPlaces);

btnFavs.addEventListener('click', async () => {
    const favs = getFavs();
    if (favs.length === 0) { alert('No hay favoritos.'); return; }
 
    let startLat = null, startLng = null;
    try {
        const pos = await new Promise((ok, err) => navigator.geolocation.getCurrentPosition(ok, err, { timeout: 3000 }));
        startLat = pos.coords.latitude; startLng = pos.coords.longitude;
    } catch (_) { }

    const res = await fetch('/api/places/itinerary', {
        method: 'POST', headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ placeIds: favs, startLat, startLng })
    });
    const route = await res.json();

    clearMarkers();
    const latlngs = [];
    route.forEach(p => {
        addMarker(p);
        latlngs.push([p.latitude, p.longitude]);
    });
    if (latlngs.length > 1) {
        const poly = L.polyline(latlngs, { weight: 5 }).addTo(map);
        map.fitBounds(poly.getBounds(), { padding: [30, 30] });
    } else {
        map.setView(latlngs[0], 17);
    }
});

renderFavs();
loadPlaces();
