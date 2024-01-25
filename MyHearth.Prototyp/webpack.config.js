const path = require("path");
const webpack = require("webpack");
//const HtmlWebpackPlugin = require("html-webpack-plugin");
const VueLoaderPlugin = require("vue-loader/lib/plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
//const ExtractTextPlugin = require('extract-text-webpack-plugin');
//const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
//const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const bundleOutputDir = "./wwwroot/dist";

module.exports = (env, argv) => {
    const isDevBuild = !(argv && argv.mode === "production");

    console.log(
        "APP - WebPack is running in " +
        (isDevBuild ? "DEVELOPMENT" : "PRODUCTION") +
        " ENV:" +
        env
    );

    return [
        {
            mode: isDevBuild ? "development" : "production",
            stats: { modules: false },
            context: __dirname,
            entry: {
                main: ["./ClientApp/boot-app.js"]
            },
            devtool: isDevBuild ? "eval-source-map" : "",
            resolve: {
                extensions: [".js", ".ts", ".vue"],
                alias: {
                    "@components": path.resolve(
                        __dirname,
                        "./ClientApp/components"
                    ),
                    "@backend": path.resolve(__dirname, "./ClientApp/backend"),
                    "@utils": path.resolve(__dirname, "./ClientApp/utils")
                }
            },

            module: {
                rules: [
                    {
                        test: /\.vue$/,
                        include: /ClientApp/,
                        loader: "vue-loader",
                        options: { loaders: { js: "ts-loader" } }
                    },
                    {
                        test: /\.ts$/,
                        loader: "ts-loader",
                        exclude: /node_modules/,
                        include: /ClientApp/,
                        options: { appendTsSuffixTo: [/\.vue$/], transpileOnly: true }
                    },
                    {
                        test: /\.js$/,
                        include: /ClientApp/,
                        use: "babel-loader"
                    },
                    {
                        test: /\.css$/,
                        use: isDevBuild
                            ? ["vue-style-loader", "css-loader"]
                            : [MiniCssExtractPlugin.loader, "css-loader"]
                    }, // MiniCssExtractPlugin.loader , 'css-loader'  "vue-style-loader"
                    {
                        test: /\.scss$/,
                        use: [
                            {
                                loader:
                                    "vue-style-loader" /* creates style nodes from JS strings */
                            },
                            {
                                loader:
                                    "css-loader" /* translates CSS into CommonJS */
                            },
                            {
                                loader: "sass-loader" /* compiles Sass to CSS*/,
                                options: {
                                    implementation: require('sass'),
                                    fiber: require('fibers'),
                                    indentedSyntax: true // optional
                                }
                            }
                        ]
                    },

                    { test: /\.(png|woff|woff2|eot|ttf|svg)$/, loader: 'url-loader?limit=100000' }

                    //{
                    //    test: /\.(png|jpg|jpeg|gif|svg)$/,
                    //    use: "url-loader?limit=25000"
                    //}
                ]
            },
            output: {
                filename: "[name].js",
                path: path.join(__dirname, bundleOutputDir),
                publicPath: "/dist/"
            },
            plugins: [
                // Define plugin can set the enviroment development/production
                new webpack.DefinePlugin({
                    "process.env": {
                        NODE_ENV: JSON.stringify(
                            isDevBuild ? "development" : "production"
                        )
                    }
                }),

                // Extracting CSS in separate file
                new MiniCssExtractPlugin({
                    filename: "main.css"
                }),
                new VueLoaderPlugin(),
                new webpack.DllReferencePlugin({
                    context: __dirname,
                    manifest: require("./wwwroot/dist/vendor-manifest.json")
                })
            ].concat(
                isDevBuild
                    ? [
                        // Plugins that apply in development builds only
                        new webpack.SourceMapDevToolPlugin({
                            filename: "[file].map", // Remove this line if you prefer inline source maps
                            moduleFilenameTemplate: path.relative(
                                bundleOutputDir,
                                "[resourcePath]"
                            ) // Point sourcemap entries to the original file locations on disk
                        })
                    ]
                    : [
                        // Plugins that apply in production builds only
                        // new UglifyJsPlugin(),
                    ]
            )
        }
    ];
};
