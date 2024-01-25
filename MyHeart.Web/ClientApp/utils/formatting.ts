import moment from 'moment';

export default {
    formatDateAsDistance(datetime: Date) {
        console.log(datetime);
        return this.formatSecondsAsDistance(Math.floor((datetime - Date.now()) / 1000));
    },

    formatSecondsAsDistance(seconds: number) {
        console.log(seconds);
        if (seconds == 0) return "nyní";
        let units = [
            { seconds: 365 * 24 * 60 * 60, past: ["před rokem", "před # lety"], future: ["za rok", "za 2 roky", "za 3 roky", "za 4 roky", "za # let"] },
            { seconds: 30 * 24 * 60 * 60, past: ["před měsícem", "před # měsíci"], future: ["za měsíc", "za 2 měsíce", "za 3 měsíce", "za 4 měsíce", "za # měsíců"] },
            { seconds: 7 * 24 * 60 * 60, past: ["před týdnem", "před # týdny"], future: ["za týden", "za 2 týdny", "za 3 týdny", "za 4 týdny", "za # týdnů"] },
            { seconds: 24 * 60 * 60, past: ["před 1 dnem", "před # dny"], future: ["za 1 den", "za 2 dny", "za 3 dny", "za 4 dny", "za # dnů"] },
            { seconds: 60 * 60, past: ["před hodinou", "před # hodinami"], future: ["za hodinu", "za 2 hodiny", "za 3 hodiny", "za 4 hodiny", "za # hodin"] },
            { seconds: 60, past: ["před minutou", "před # minutami"], future: ["za minutu", "za 2 minuty", "za 3 minuty", "za 4 minuty", "za # minut"] },
            { seconds: 1, past: ["před sekundou", "před # sekundami"], future: ["za sekundu", "za 2 sekundy", "za 3 sekundy", "za 4 sekundy", "za # sekund"] }
        ];
        let isPast = seconds < 0;
        seconds = Math.abs(seconds);
        for (let i = 0; i < units.length; i++) {
            let unit = units[i];
            if (seconds >= unit.seconds) {
                let quantity = Math.floor(seconds / unit.seconds);
                let words = isPast ? unit.past : unit.future;
                let word = words[Math.min(quantity, words.length) - 1];
                return word.replace("#", quantity + "");
            }
        }
    },

    formatDate(date, format = null) {

        if(date == null){
            return ""
        }

        if(format == null){
            format = 'DD.MM.YYYY'
        }

        return moment(date).format(format);
    }
}