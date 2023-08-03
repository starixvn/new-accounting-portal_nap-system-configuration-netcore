var webpackConfig = require('../config/webpack.config.test');

module.exports = function (config) {
    config.set({
        browsers: ['PhantomJS'],
        frameworks: ['mocha', 'sinon-chai', 'phantomjs-shim'],
        reporters: ['spec', 'coverage'],
        files: [
            '../node_modules/es6-promise/dist/es6-promise.auto.js',
            './libs/jquery.js',
            '../../../NAP.js',
            './index.ts',
             '../src/components/header-row/header-row.ts'],
        preprocessors: {
            './index.ts': ['webpack'],
            '../src/components/header-row/header-row.ts': ['sourcemap', 'coverage']
        },
        webpack: webpackConfig,
        webpackMiddleware: {
            noInfo: true
        },
        autoWatch: false,
        singleRun: true,
        colors: true,
        coverageReporter: {
            dir: './coverage',
            reporters: [
              { type: 'text-summary' },
              { type: 'html' }
            ]
        }
    })
}