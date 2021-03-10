let doc = document.body;
let figures = [...document.querySelectorAll('.mouse-move-animation')];
doc.addEventListener('mousemove', (e) => {
    let currentX = e.clientX;
    let currentY = e.clientY;
    figures.map((fig) => {
        if (fig.classList.contains('onion-animation')) {
            let leftP = 49;
            let topP = 57;
            movePicture(fig, leftP, topP, currentY, currentX);
        } else if (fig.classList.contains('carrot-animation')) {
            let leftP = 35;
            let topP = 39;
            movePicture(fig, leftP, topP, currentY, currentX);
        } else if (fig.classList.contains('pepper-animation')) {
            let leftP = 9;
            let topP = 56;
            movePicture(fig, leftP, topP, currentY, currentX);
        } else if (fig.classList.contains('avocado-animation')) {
            let leftP = 25;
            let topP = 14;
            movePicture(fig, leftP, topP, currentY, currentX);
        } else if (fig.classList.contains('cheese-animation')) {
            let leftP = 27;
            let topP = 34;
            movePicture(fig, leftP, topP, currentY, currentX);
        } else if (fig.classList.contains('leaf-1-animation')) {
            let leftP = -3;
            let topP = 10;
            movePicture(fig, leftP, topP, currentY, currentX);
        } else if (fig.classList.contains('leaf-2-animation')) {
            let leftP = 63;
            let topP = 20;
            movePicture(fig, leftP, topP, currentY, currentX);
        } else if (fig.classList.contains('leaf-3-animation')) {
            let leftP = 68;
            let topP = 6;
            movePicture(fig, leftP, topP, currentY, currentX);
        }


    });
});

function movePicture(fig, leftP, topP, currentY, currentX) {
    let screenWidth = document.documentElement.offsetWidth;
    let screenHeight = document.documentElement.offsetHeight;
    let formula = currentX / (screenWidth / 110);
    let leftPixel = screenWidth / 100 * leftP;
    let topPixel = screenHeight / 100 * topP;

    if (fig.classList.contains('reverse-mouse')) {
        let posX = (leftPixel + currentX / (screenWidth / 100)) / (screenWidth / 100);
        let posY = (topPixel - currentY / (screenHeight / 17)) / (screenHeight / 100);
        fig.style.right = posX + '%';
        fig.style.top = posY + '%';
    } else if (fig.classList.contains('follow-mouse')) {
        let posX = (leftPixel - currentX / (screenWidth / 100)) / (screenWidth / 100);
        let posY = (topPixel + currentY / (screenHeight / 17)) / (screenHeight / 100);
        fig.style.right = posX + '%';
        fig.style.top = posY + '%';
    } else if (fig.classList.contains('reverse-mouse-y')) {
        let posX = (leftPixel - currentX / (screenWidth / 100)) / (screenWidth / 100);
        let posY = (topPixel - currentY / (screenHeight / 17)) / (screenHeight / 100);
        fig.style.right = posX + '%';
        fig.style.top = posY + '%';
    } else if (fig.classList.contains('reverse-mouse-x')) {
        let posX = (leftPixel + currentX / (screenWidth / 100)) / (screenWidth / 100);
        let posY = (topPixel + currentY / (screenHeight / 17)) / (screenHeight / 100);
        fig.style.right = posX + '%';
        fig.style.top = posY + '%';
    }
}

