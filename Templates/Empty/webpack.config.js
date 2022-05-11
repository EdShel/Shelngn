const path = require("path");
const CopyPlugin = require("copy-webpack-plugin");

module.exports = (env) => ({
  entry: path.resolve(__dirname, "index.js"),
  context: __dirname,
  mode: env.production ? "production" : "development",
  output: {
    path: path.resolve(__dirname, "dist"),
    filename: "index.js",
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        {
          context: __dirname,
          from: "**/*.{jpg,png}",
          noErrorOnMissing: true,
          globOptions: {
            ignore: ['**/dist/**']
          }
        },
      ],
    }),
  ],
  resolve: {
    alias: { Shelngn: path.resolve(__dirname, 'shelngn.js') }
  }
});
