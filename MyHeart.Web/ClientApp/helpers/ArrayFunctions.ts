export default class ArrayFunctions{
    static diff(firstColecttion, secondCollection){
        return firstColecttion.filter(item => secondCollection.indexOf(item) < 0);
    }
}