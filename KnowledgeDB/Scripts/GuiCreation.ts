export function replaceButtonWithImage(button, imageClasses) : HTMLElement {
    const buttonParent : HTMLElement = button.parentElement;
    if (buttonParent != null) {
        const image :HTMLElement = document.createElement('i')
        image.className = imageClasses

        buttonParent.insertBefore(image, button);
        buttonParent.removeChild(button);

        return image;
    }
}

export function changeImage(image, imageClasses): void{
    if (image != null) {
        image.className = imageClasses;
    }
}

export function createImageButton(imageClasses, buttonClasses, buttonTitle): HTMLButtonElement {
    let button: HTMLButtonElement = document.createElement('button');
    let image: HTMLElement= document.createElement('i');

    image.className = imageClasses;
    button.title = buttonTitle;
    button.className = buttonClasses
    button.appendChild(image);

    return button;
}

function scaleMainContent(colSize) {
    let main = document.getElementById("main");
    let classes = main.className.split(" ");

    for (var i = 0; i < classes.length; i++) {
        if (classes[i].includes("col")) {
            main.classList.remove(classes[i]);
        }
    }

    main.classList.add(colSize);
}


export function toggleSideBarExpansion(dessiredShrinkedClass?: string, dessiredDefaultClass?: string ) {
    let shrinkedClass = "col-md-8";
    let defaultClass = "col-md-10";

    let main = document.getElementById("main");

    if (dessiredShrinkedClass != null) {
        shrinkedClass = dessiredShrinkedClass;
    }

    if (dessiredDefaultClass != null) {
        defaultClass = dessiredDefaultClass;
    }

    if (main == null) {
        return
    }

    if (main.classList.contains("shrinked")) {
        scaleMainContent(defaultClass);
    } else {
        scaleMainContent(shrinkedClass);
    }

    main.classList.toggle("shrinked");
}