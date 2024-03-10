const path = require('path');

module.exports = {
    entry: {
        bridge: './bridge.ts',
        models: './models.ts',
        utils: './utils.ts'
    },
    target: 'node',
    module: {
        rules: [
            {
                test: /\.ts$/,
                use: 'ts-loader',
                exclude: /node_modules/,
            },
        ],
    },
    resolve: {
        extensions: ['.ts', '.js'],
    },
    output: {
        filename: '[name].js',
        path: path.resolve(__dirname, 'js'),
        libraryTarget: 'commonjs2',
    },
    optimization: {
        usedExports: true,
    },
    mode: 'production',
};