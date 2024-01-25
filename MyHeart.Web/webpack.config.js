const path = require("path");
const webpack = require("webpack");
//const HtmlWebpackPlugin = require("html-webpack-plugin");
const VueLoaderPlugin = require("vue-loader/lib/plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
//const ExtractTextPlugin = require('extract-text-webpack-plugin');
//const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
//const UglifyJsPlugin = require('uglifyjs-webpack-plugin');
const bundleOutputDir = "./wwwroot/dist";
const getter = require("./ClientApp/backend/ApiUrlGetter.js");

module.exports = (env, argv) => {
  console.log(env);
  console.log(argv);
  const isDevBuild = !env || env == "dev"//argv && argv.mode === "development";
  console.log(getter);
  var url = getter.getApiUrl(env);
  console.log(url);

  console.log(
    "APP - WebPack is running in " +
    (isDevBuild ? "DEVELOPMENT" : "PRODUCTION") +
    " ENV:" +
    env
  );



  return [
    {
      mode: isDevBuild ? "development" : "production",
      stats: {
        modules: false,
      },
      context: __dirname,
      entry: {
        main: ['webpack-hot-middleware/client', './ClientApp/boot-app.js'],
      },
      devtool: isDevBuild ? "eval-source-map" : "",
      resolve: {
        extensions: [".js", ".ts", ".vue", ".jpg", ".jpeg", ".png"],
        alias: {
          "@components": path.resolve(__dirname, "./ClientApp/components"),
          "@backend": path.resolve(__dirname, "./ClientApp/backend"),
          "@utils": path.resolve(__dirname, "./ClientApp/utils"),
          "@helpers": path.resolve(__dirname, "./ClientApp/helpers"),
          "@resources": path.resolve(__dirname, "./ClientApp/resources"),
          "@models": path.resolve(__dirname, "./ClientApp/models"),
          "@constants": path.resolve(__dirname, "./ClientApp/constants"),
        },
      },
      module: {
        rules: [
          {
            test: /\.vue$/,
            include: /ClientApp/,
            loader: "vue-loader",
            options: {
              loaders: {
                js: "ts-loader",
              },
            },
          },
          {
            test: /\.ts$/,
            loader: "ts-loader",
            exclude: /node_modules/,
            include: /ClientApp/,
            options: {
              appendTsSuffixTo: [/\.vue$/],
              transpileOnly: true,
            },
          },
          {
            test: /\.js$/,
            include: /ClientApp/,
            use: "babel-loader",
          },
          {
            test: /\.css$/,
            use: isDevBuild
              ? ["vue-style-loader", "css-loader"]
              : [MiniCssExtractPlugin.loader, "css-loader"],
          }, // MiniCssExtractPlugin.loader , 'css-loader'  "vue-style-loader"
          {
            test: /\.s(c|a)ss$/,
            use: [
              "vue-style-loader",
              "css-loader",
              {
                loader: "sass-loader",
                options: {
                  implementation: require("sass"),
                  indentedSyntax: true, // optional
                },
              },
            ],
          },
          {
            test: /\.(png|jpg|jpeg|gif)$/,
            use: "url-loader?limit=25000",
          },
          {
            test: /\.(woff(2)?|ttf|eot)(\?v=\d+\.\d+\.\d+)?$/,
            use: [
              {
                loader: "file-loader",
                options: {
                  name: "[name].[ext]",
                  outputPath: "fonts/",
                },
              },
            ],
          },
          {
            test: /\.styl$/,
            use: [
              {
                loader: "vue-style-loader", // creates style nodes from JS strings
              },
              {
                loader: "css-loader", // translates CSS into CommonJS
              },
              {
                loader: "stylus-loader", // compiles Stylus to CSS
              },
            ],
          },
          {
            test: /\.svg$/,
            loader: 'vue-svg-loader',
          },
        ],
      },
      output: {
        filename: "[name].js",
        path: path.join(__dirname, bundleOutputDir),
        publicPath: "/dist/",
      },
      plugins: [
        // Define plugin can set the enviroment development/production
        new webpack.DefinePlugin({
          "process.env": {
            NODE_ENV: JSON.stringify(isDevBuild ? "development" : "production"),
            MODE: JSON.stringify(env),
            BASE_URL: JSON.stringify(url),
          },
        }),
        new webpack.ProvidePlugin({
          $: 'jquery',
          jQuery: 'jquery'
        }),
        // Extracting CSS in separate file
        new MiniCssExtractPlugin({
          filename: "main.css",
        }),
        new VueLoaderPlugin(),
        new webpack.HotModuleReplacementPlugin(),
        //new webpack.DllReferencePlugin({
        //  context: __dirname,
        //}),
      ].concat(
        isDevBuild
          ? [
            // Plugins that apply in development builds only
            new webpack.SourceMapDevToolPlugin({
              filename: "[file].map", // Remove this line if you prefer inline source maps
              moduleFilenameTemplate: path.relative(
                bundleOutputDir,
                "[resourcePath]"
              ), // Point sourcemap entries to the original file locations on disk
            }),
          ]
          : [
            // Plugins that apply in production builds only
            // new UglifyJsPlugin(),
          ]
      ),
    },
  ];
};
