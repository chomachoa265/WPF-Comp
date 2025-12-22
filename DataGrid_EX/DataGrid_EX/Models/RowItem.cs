using CommunityToolkit.Mvvm.ComponentModel;

namespace DataGrid_EX.Models
{
    // 繼承 ObservableObject 以支援屬性變更通知
    public partial class RowItem : ObservableObject
    {
        [ObservableProperty]
        private int _id; // 這就是我們要自動更新的 ROW ID

        [ObservableProperty]
        private string _name = string.Empty;

        // 新增一個數值欄位，用來演示平均值計算
        [ObservableProperty]
        private double _score = 0.0f;

        [ObservableProperty]
        private string _description = string.Empty;

        // 這裡可以根據需要擴充更多屬性，或者使用 Dictionary 來存放動態欄位值
    }
}
