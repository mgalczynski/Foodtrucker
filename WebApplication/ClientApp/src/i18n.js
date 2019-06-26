import i18n from 'i18next';
import LanguageDetector from 'i18next-browser-languagedetector';
import {initReactI18next} from 'react-i18next';

i18n
    .use(LanguageDetector)
    .use(initReactI18next)
    .init({
        // we init with resources
        resources: {
            en: {
                translations: {}
            },
            pl: {
                translations: {
                    'Current password': 'Aktualne hasło',
                    'New password': 'Nowe hasło',
                    'Change password': 'Zmień hasło',
                    'Close': 'Zamknij',
                    'Are you an employee or owner of foodtruck, click here to add your foodtruck to this app':'Czy jesteś pracownikiem albo właścicielem foodtrucka? Kliknij tutaj aby dodać swój foodtruck'
                }
            }
        },
        fallbackLng: 'en',
        debug: true,

        // have a common namespace used around the full app
        ns: ['translations'],
        defaultNS: 'translations',

        keySeparator: false, // we use content as keys

        interpolation: {
            escapeValue: false, // not needed for react!!
            formatSeparator: ','
        },

        react: {
            wait: true
        }
    });

export default i18n;