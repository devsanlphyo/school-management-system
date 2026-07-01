const CACHE_NAME = 'schools-mgmt-cache-v1';
const OFFLINE_URL = 'offline.html';

const urlsToCache = [
  '/',
  '/manifest.json',
  '/app.css',
  '/MainSchoolsManagementSystem.styles.css',
  OFFLINE_URL
];

self.addEventListener('install', event => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => {
        return cache.addAll(urlsToCache);
      })
  );
});

self.addEventListener('activate', event => {
  const cacheWhitelist = [CACHE_NAME];
  event.waitUntil(
    caches.keys().then(cacheNames => {
      return Promise.all(
        cacheNames.map(cacheName => {
          if (cacheWhitelist.indexOf(cacheName) === -1) {
            return caches.delete(cacheName);
          }
        })
      );
    })
  );
});

self.addEventListener('fetch', event => {
  // We only want to handle GET requests
  if (event.request.method !== 'GET') return;

  event.respondWith(
    fetch(event.request)
      .catch(() => {
        // If the network fails, we look in the cache
        return caches.match(event.request)
          .then(response => {
            // Return cached response if found
            if (response) {
              return response;
            }
            // If it's a navigation request, try to return offline page
            if (event.request.mode === 'navigate') {
              return caches.match(OFFLINE_URL).then(offlineResponse => {
                 return offlineResponse || new Response('Offline - No internet connection.', { status: 503, statusText: 'Service Unavailable' });
              });
            }
            // For all other requests that fail network and aren't cached
            return new Response('Network error occurred', { status: 408, statusText: 'Request Timeout' });
          });
      })
  );
});
