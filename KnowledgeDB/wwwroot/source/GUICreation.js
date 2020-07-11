function replaceButtonWithImage(button, imageClasses) {
    const buttonParent = button.parentElement;
    if (buttonParent != null) {
        const image = document.createElement('i')
        image.className = imageClasses

        buttonParent.insertBefore(image, button);
        buttonParent.removeChild(button);

        return image;
    }
}

function ChangeImage(image, imageClasses) {
    if (image != null) {
        image.className = imageClasses;
    }
}

function createImageButton(imageClasses, buttonClasses, buttonTitle) {
    let button = document.createElement('button');
    let image = document.createElement('i');

    image.className = imageClasses;
    button.title = buttonTitle;
    button.className = buttonClasses
    button.appendChild(image);

    return button;
}