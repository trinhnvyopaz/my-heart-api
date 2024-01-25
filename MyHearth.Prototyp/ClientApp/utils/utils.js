export function getModeIcon(mode, active)
{
    return "'../img/mode-manual-active.png'";
    var path = '../img/mode-';
    switch (mode)
    {
        case 1:
            path += 'scheduled';
            break;
        case 2:
            path += 'weather';
            break;
        default:
            path += 'manual';
            break;
    }
    path += '-';
    if (active)
    {
        path += 'active';
    }
    else
    {
        path += 'inactive';
    }
    path += '.png'
    return path;
}

//const CULTURE_KEY = 'culture_key';

//export function setCulture(culture)
//{
//    console.log('Saving culture', culture);
//    localStorage.setItem(CULTURE_KEY, culture);
//}

//export function getCulture()
//{
//    var culture = localStorage.getItem(CULTURE_KEY);
//    console.log('Loading culture', culture);
//    return culture;
//}

export function formatDate(date)
{
    var year = date.substring(0, 4);
    var month = date.substring(5, 7);
    var day = date.substring(8, 10);

    var hour = date.substring(11, 13);
    var minute = date.substring(14, 16);

    return hour + ':' + minute + ' ' + day + '/' + month + '/' + year;
}

export function decodeFlag(flag, modelType)
{
    //console.log('DECODING FLAG: ' + flag + ', MODEL: ' + modelType);

    var userModelArray =
        [
            'DuplicateUsername',
            'DuplicateEmail',
            'UserNotExists',
            'MailQueueError',
            'Unknown',
            'Unknown',
            'EmptyUsername',
            'InvalidUsername',
            'EmptyEmail',
            'InvalidEmail',
            'EmptyPassword',
            'InvalidPassword',
            'PasswordsNotMatch',
            'InvalidBirthday',
            'TermsNotAgreed',
            'AlreadyConfirmed',
            'WrongPassword',
            'EmptyOldPassword'
        ];

    var deviceModelArray =
        [
            'DuplicateName',            //  0
            'DeviceNotExists',          //  1
            'MailQueueError',           //  2
            'EmptyName',                //  3
            'InvalidName',              //  4
            'EmptyDeviceId',            //  5
            'InvalidDeviceId',          //  6
            'AlreadyRequested',         //  7
            'AlreadyRequested',         //  8
            'AlreadyRegistered',        //  9
            'InvalidStartTime',         //  10
            'MaxRunTimeExceeded',       //  11
            'InvalidTimezone',          //  12
            'InvalidCoordinates'        //  13
        ];

    var deviceWarningModel =
        [
            'NO_WARNING',               //  0
            'UNDER_VOLTAGE',            //  1
            'NO_FLUX',                  //  2
            'PRESSURE_FAIL',            //  3
            'MOTOR_OVER_CURRENT',       //  4
            'MOTOR_FAIL',               //  5
            'CONTROLLER_FAIL',          //  6
            'OVERHEAT',                 //  7
            'LOW_TEMP'                  //  8
        ];


    if (flag > 0x7fffffff || flag < -0x80000000)
    {
        return null;
    }

    var result = [];
    var modelArray = [];

    for (var nShifted = flag, boolArray = []; nShifted;
        boolArray.push(Boolean(nShifted & 1)), nShifted >>>= 1);

    switch (modelType)
    {
        case 'userModel':
            modelArray = userModelArray;
            break;

        case 'deviceModel':
            modelArray = deviceModelArray;
            break;
        case 'deviceWarningModel':
            modelArray = deviceWarningModel;
            break;

        default:
            return result;
    }

    for (var index = 0; index < boolArray.length; index++)
    {
        if (boolArray[index])
        {
            result.push(modelArray[index]);
        }
    }

    console.log('FLAGS', result);

    return result;
}
