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
            // Return cached response if found, otherwise return the offline page
            // especially for navigation requests
            if (response) {
              return response;
            }
            if (event.request.mode === 'navigate') {
              return caches.match(OFFLINE_URL);
            }
          });
      })
  );
});
