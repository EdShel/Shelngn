const path = require("path");
const CopyPlugin = require("copy-webpack-plugin");

module.exports = (env) => ({
  entry: path.resolve(__dirname, "index.js"),
  mode: "production",
  output: {
    path: path.resolve(__dirname, "dist"),
    filename: "index.js",
  },
  plugins: [
    new CopyPlugin({
      patterns: [
        {
          context: env.workspace + "/",
          from: "**/*.{jpg,png}",
          noErrorOnMissing: true,
          globOptions: {
            ignore: ['**/dist/**']
          }
        },
      ],
    }),
  ],
});
