const path = require('path');
const { VueLoaderPlugin } = require('vue-loader')

console.log('destination is ' + path.resolve(__dirname, 'web'));
module.exports = {
    entry: './web_src/app.js',
    output: {
        path: path.resolve(__dirname, 'web'),
        filename: 'walldisplay.js'
    },
    module: {
        rules: [{
            test: /\.less$/,
            use: [{
                loader: 'style-loader'
            }, {
                loader: 'css-loader'
            }, {
                loader: 'less-loader'
            }]
        }, {
            test: /\.vue$/,
            use: 'vue-loader'
        }, {
            test: /\.(woff(2)?|ttf|eot|svg)(\?v=\d+\.\d+\.\d+)?$/,
            use: [{
                loader: 'file-loader',
                options: {
                    name: '[name].[ext]',
                    outputPath: 'fonts/'
                }
            }]
        }, {
            test: /\.(jpe?g|png)(\?[a-z0-9=&.]+)?$/,
            use: 'base64-inline-loader?limit=1000&name=[name].[ext]'
        }]
    },
    plugins: [
        new VueLoaderPlugin()
    ]
};