export default {
    clamp(value, min, max) {
        if (!value)
            return min;
        if (isNaN(value))
            return min;
        if (value < min)
            return min;
        if (value > max)
            return max;
        return value;
    },
}