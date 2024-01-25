const path = require("path");
const webpack = require("webpack");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const bundleOutputDir = "./wwwroot/dist";
const getter = require("./ClientApp/backend/ApiUrlGetter.js");

module.exports = (env, argv) => {
  const isDevBuild = argv && argv.mode === "development";
  console.log(getter);

  var url = getter.getApiUrl(env);
  console.log(url);

  console.log(
    "VENDOR - WebPack is running in " +
      (isDevBuild ? "DEVELOPMENT" : "PRODUCTION") +
      " ENV:" +
      env
  );

  return [
    {
      stats: {
        modules: false,
      },
      resolve: {
        extensions: [".js"],
      },
      context: __dirname,
      entry: {
        vendor: [
          "event-source-polyfill",
          "vue",
          "vuex",
          "vue-router",
          "jquery",
        ],
      },
      module: {
        rules: [
          {
            test: /\.css(\?|$)/,
            use: [MiniCssExtractPlugin.loader, "css-loader"],
          }, // MiniCssExtractPlugin.loader,
          {
            test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/,
            use: "url-loader?limit=100000",
          },
        ],
      },
      output: {
        path: path.join(__dirname, "wwwroot", "dist"),
        publicPath: "dist/",
        filename: "[name].js",
        library: "[name]_[hash]",
      },
      plugins: [
        new MiniCssExtractPlugin({
          filename: "vendor.css",
        }),
        new webpack.ProvidePlugin({
          $: "jquery",
          jQuery: "jquery",
        }), // Maps these identifiers to the jQuery package (because Bootstrap expects it to be a global variable)
        new webpack.DefinePlugin({
          "process.env.NODE_ENV": JSON.stringify(
            isDevBuild ? "development" : "production"
          ),
          "process.env.BASE_URL": JSON.stringify(url),
        }),
        //new webpack.DllPlugin({
        //  path: path.join(__dirname, "wwwroot", "dist", "[name]-manifest.json"),
        //  name: "[name]_[hash]",
        //}),
      ].concat(isDevBuild ? [] : []),
    },
  ];
};
