export default function Chat() {
  return (
    <view className="bg-slate-700 flex flex-col gap-2 p-2 rounded-lg fixed bottom-2 left-2 min-w-96 text-white">
      <scroll
        className="flex-1 flex flex-col-reverse min-h-32 w-full p-0 rounded-md bg-black bg-opacity-50"
        direction="vertical"
      >
        <view className="p-2">
          <text>Scrollable Text</text>
        </view>
      </scroll>
      <view className="flex flex-row gap-2 text-sm">
        <button className="bg-slate-800 shadow-inner ring-1 shadow-slate-700 p-2">
          <text>Say</text>
        </button>
        <input className="bg-black bg-opacity-50 rounded-md flex-1" />
      </view>
    </view>
  )
}
