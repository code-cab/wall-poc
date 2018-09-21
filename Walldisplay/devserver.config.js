module.exports = {
    proxy: {
        '/api*': 'http://127.0.0.1:9000/'
    },
    publicPaths: {
        '/': 'web'
    },
    port: 9010,
};