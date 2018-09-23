const fs = require('fs');

module.exports = {
    directory: 'web',
    port: 9010,
    mocks: MockBase => class ApiMock extends MockBase {
        mocks(options) {
            return[{
                route: '/api/:id',
                responses: [{
                    request: {
                        method: 'GET',
                        id: 'json'
                    },
                    response: function(ctx, id) {
                        ctx.json = 'json';
                        let data = fs.readFileSync('./web_src/mock/api/' + id + '.json', 'utf-8');
                        ctx.body = JSON.parse(data);
                    }
                }]
            }]
        }
    }
};