using BookingService.Application.Dto;
using BookingService.Application.Services.Interfaces;
using Common.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BookingService.Application.Services;

public class InventoryClient : IInventoryClient
{
    private readonly HttpClient _http;

    public InventoryClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<HotelSummary?> GetHotelSummaryAsync(Guid hotelId)
    {
        var response = await _http.GetAsync($"/api/v1/hotels/{hotelId}/info");
        if (!response.IsSuccessStatusCode)
            throw new ArgumentException(response.ReasonPhrase);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<HotelSummary>>();
        return apiResponse?.Data;
    }


    public async Task<ReservationReceipt> ReserveRoomAsync(ReservationRequest body)
    {
        var response = await _http.PostAsJsonAsync("/api/v1/hotels/reserve", body);
        var apiResponse = response.Content.ReadFromJsonAsync<ApiResponse<ReservationReceipt>>();

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(
                $"Inventory service returned {response.StatusCode}: {apiResponse.Result?.Message}");

        return apiResponse.Result.Data;
    }
}