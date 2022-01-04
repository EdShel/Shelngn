let currentScene = createScene();

export function createScene() {
    let entities = [];
    let drawOrder = [];

    const update = () => {

    };

    const updateLists = () => {
        entities = entities.filter(e => !e._data.toBeDeleted);
        drawOrder = entities.filter(e => e.draw).sort(e => e.z);
    };

    const render = () => {
        for (const drawable of drawOrder) {
            drawable.draw();
        }
    };

    return {
        entities,
        update,
        render
    };
}

export const instanceCreate = newInstance => {
    newInstance._data = {
        toBeDeleted: false
    }
    currentScene.entities.push(newInstance);
}

export const instanceDestroy = instance => {
    instance._data.toBeDeleted = true;
}