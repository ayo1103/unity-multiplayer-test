#if USE_DATA_CACHING
let cacheName = {{{JSON.stringify(COMPANY_NAME + "-" + PRODUCT_NAME + "-" + PRODUCT_VERSION )}}};
let currentCacheName = cacheName;

async function updateCacheName() {
    try {
        const response = await fetch('Build/version.json');
        const versionData = await response.json();
        currentCacheName = `${cacheName}-${versionData.version}`;
    } catch (error) {
        console.error('Error fetching version.json:', error);
    }
}

const contentToCache = [
    "Build/{{{ LOADER_FILENAME }}}",
    "Build/{{{ FRAMEWORK_FILENAME }}}",
#if USE_THREADS
    "Build/{{{ WORKER_FILENAME }}}",
#endif
    "Build/{{{ DATA_FILENAME }}}",
    "Build/{{{ CODE_FILENAME }}}",
    "TemplateData/style.css"
];
#endif

self.addEventListener('install', function (e) {
    console.log('[Service Worker] Install');

#if USE_DATA_CACHING
    e.waitUntil((async function () {
        await updateCacheName();
        const cache = await caches.open(currentCacheName);
        console.log('[Service Worker] Caching all: app shell and content');
        await cache.addAll(contentToCache);
    })());
#endif
});

#if USE_DATA_CACHING
self.addEventListener('fetch', function (e) {
    e.respondWith((async function () {
        let response = await caches.match(e.request);
        console.log(`[Service Worker] Fetching resource: ${e.request.url}`);
        if (response) { return response; }

        response = await fetch(e.request);
        const cache = await caches.open(currentCacheName);
        console.log(`[Service Worker] Caching new resource: ${e.request.url}`);
        cache.put(e.request, response.clone());
        return response;
    })());
});
#endif

self.addEventListener('activate', function (e) {
    console.log('[Service Worker] Activate');
    e.waitUntil((async function () {
        await updateCacheName();
        const cacheNames = await caches.keys();
        await Promise.all(
            cacheNames.map((name) => {
                if (name !== currentCacheName) {
                    console.log(`[Service Worker] Deleting old cache: ${name}`);
                    return caches.delete(name);
                }
            })
        );
    })());
});
