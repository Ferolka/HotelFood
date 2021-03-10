

var compatibleDevices = [
    {
        deviceName: 'ACR122U USB NFC Reader',
        productId: 0x2200,
        vendorId: 0x072f,
        thumbnailURL: 'images/acr122u.png'
    },
    {
        deviceName: 'SCL3711 Contactless USB Smart Card Reader',
        productId: 0x5591,
        vendorId: 0x04e6,
        thumbnailURL: 'images/scl3711.png'
    }
]



function log(message, object) {
    console.log(message);
    console.log(object);
    return;
    var logArea = document.querySelector('.logs');
    var pre = document.createElement('pre');
    pre.textContent = message;
    if (object)
        pre.textContent += ': ' + JSON.stringify(object, null, 2) + '\n';
    logArea.appendChild(pre);
    logArea.scrollTop = logArea.scrollHeight;
    document.querySelector('#logContainer').classList.remove('small');
}

function handleDeviceTimeout(func, args) {
    var timeoutInMs = 1000;
    var hasTags = false;
    setTimeout(function () {
        if (!hasTags) {
            log('Timeout! No tag detected');
        }
    }, timeoutInMs);
    var args = args || [];
    args = args.concat([function () { hasTags = true; }]);
    func.apply(this, args);
}

function onReadNdefTagButtonClicked() {
    handleDeviceTimeout(readNdefTag);
}

function readNdefTag(callback) {
    chrome.nfc.read(device, {}, function (type, ndef) {
        log('Found ' + ndef.ndef.length + ' NFC Tag(s)');
        for (var i = 0; i < ndef.ndef.length; i++)
            log('NFC Tag', ndef.ndef[i]);
        callback();
    });
}

function onReadMifareTagButtonClicked() {
    handleDeviceTimeout(readMifareTag);
}
function readMifareTagEx(callback) {
    var blockNumber = 0; // starting logic block number.
    var blocksCount = 2; // logic block counts.
    chrome.nfc.read_logic(device, blockNumber, blocksCount, function (rc, data) {
        log('Mifare Classic Tag', UTIL_BytesToHex(data));
        if (callback)
            callback(rc, data);
    });
}
function readMifareTag(callback) {
    var blockNumber = 0; // starting logic block number.
    var blocksCount = 2; // logic block counts.
    chrome.nfc.read_logic(device, blockNumber, blocksCount, function (rc, data) {
        log('Mifare Classic Tag', UTIL_BytesToHex(data));
        callback();
    });
}

function onWriteNdefTagButtonClicked() {
    var ndefType = document.querySelector('#write-ndef-type').value;
    var ndefValue = document.querySelector('#write-ndef-value').value;
    handleDeviceTimeout(writeNdefTag, [ndefType, ndefValue]);
}

function writeNdefTag(ndefType, ndefValue, callback) {
    var ndef = {};
    ndef[ndefType] = ndefValue;
    chrome.nfc.write(device, { "ndef": [ndef] }, function (rc) {
        if (!rc) {
            log('NFC Tag written!');
        } else {
            log('NFC Tag write operation failed', rc);
        }
        callback();
    });
}

function onWriteMifareTagButtonClicked() {
    try {
        var mifareData = JSON.parse(document.querySelector('#mifare-data').value);
        handleDeviceTimeout(writeMifareTag, [mifareData]);
    }
    catch (e) {
        log('Error', 'Mifare Data is not an Array.');
    }
}

function writeMifareTag(mifareData, callback) {
    var data = new Uint8Array(mifareData);
    var blockNumber = 0; // starting logic block number.
    chrome.nfc.write_logic(device, 0, data, function (rc) {
        if (!rc) {
            log('Mifare Tag written!');
        } else {
            log('Mifare Tag write operation failed', rc);
        }
        callback();
    });
}

function onEmulateTagButtonClicked() {
    var ndefType = document.querySelector('#emulate-ndef-type').value;
    var ndefValue = document.querySelector('#emulate-ndef-value').value;
    handleDeviceTimeout(emulateTag, [ndefType, ndefValue]);
}

function emulateTag(ndefType, ndefValue, callback) {
    var ndef = {};
    ndef[ndefType] = ndefValue;
    chrome.nfc.emulate_tag(device, { "ndef": [ndef] }, function (rc) {
        if (!rc) {
            log('NFC Tag emulated!');
        } else {
            log('NFC Tag emulate operation failed', rc);
        }
        callback();
    });
}
