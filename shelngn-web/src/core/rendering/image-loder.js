// import { assertParamNotFalse } from "../errors";

// let imagesMap = {};

// export const loadImage = (imageUrl) => {
//   assertParamNotFalse("imageUrl", imageUrl);

//   const existingImageEntry = imagesMap[imageUrl];
//   if (existingImageEntry) {
//     if (existingImageEntry.loadingPromise) {
//       return existingImageEntry.loadingPromise;
//     }
//     return Promise.resolve(existingImageEntry.image);
//   }

//   const newImage = new Image();
//   const newImageEntry = {
//     image: newImage,
//     loadingPromise: new Promise((resolve, reject) => {
//       newImage.onload = function () {
//         delete newImageEntry.loadingPromise;
//         resolve(newImage);
//       };
//       newImage.onerror = function (e) {
//         delete newImageEntry.loadingPromise;
//         reject("Error loading the image for texture: " + JSON.stringify(e));
//       };
//     }),
//   };
//   imagesMap[imageUrl] = newImageEntry;
//   newImage.src = imageUrl;
//   return newImageEntry.loadingPromise;
// };

// export const getLoadedImageOrNull = (imageUrl) => {
//   const existingImageEntry = imagesMap[imageUrl];
//   return !existingImageEntry || existingImageEntry.loadingPromise
//     ? null
//     : existingImageEntry.image;
// };
