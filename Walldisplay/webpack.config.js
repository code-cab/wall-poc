const path = require('path');
const { VueLoaderPlugin } = require('vue-loader')

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
        }]
    },
    plugins: [
        new VueLoaderPlugin()
    ]
};